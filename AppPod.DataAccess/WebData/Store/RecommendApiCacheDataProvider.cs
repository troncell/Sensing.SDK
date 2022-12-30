using AppPod.Data.DataProviders;
using AppPod.DataAccess.WebData.Downloads;
using AppPod.Setting;
using LogService;
using Newtonsoft.Json;
using Sensing.SDK;
using Sensing.SDK.Contract;
using SensingBase.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppPod.DataAccess.WebData.Store
{
    public class RecommendApiCacheDataProvider : SingleCachedListDataProviderBase<MetaPhysicsDto>
    {
        protected readonly IBizLogger logger = ServerLogFactory.GetLogger(typeof(ProductsApiCacheDataProvider));

        private SensingWebClient _webClient;
        private Settings _settings;
        public RecommendApiCacheDataProvider(SensingWebClient webClient, string metaBaseFolder, Settings settings, DownloadAlgorithm algorithm = DownloadAlgorithm.WebClient)
            : base(metaBaseFolder, "MetaPhysics.json", algorithm)
        {
            _webClient = webClient;
            _settings = settings;
        }

        public override IList<DownloadLink> ExtractLinks(string baseUrl)
        {
            if (Data == null) return null;

            baseUrl = SensingWebClient.RecommendApiHost;
            List<DownloadLink> links = new List<DownloadLink>();
            foreach (var item in Data)
            {
                if (item.LogoUrl != null)
                {
                    if (!links.Any(link => link.RelativeFileName == item.LogoUrl))
                    {
                        links.Add(new DownloadLink { Host = baseUrl, RelativeFileName = item.LogoUrl, Type = "meta" });
                    }
                }
                if (item.PicUrl != null)
                {
                    if (!links.Any(link => link.RelativeFileName == item.PicUrl))
                    {
                        links.Add(new DownloadLink { Host = baseUrl, RelativeFileName = item.PicUrl, Type = "meta" });
                    }
                }
            }
            TotalDownloadCount += links.Count;
            logger.Debug($"Ads Download Source Total is {TotalDownloadCount}");
            return links;
        }

        public override async Task<List<MetaPhysicsDto>> ReadDataFromWeb()
        {

            var pageSize = 300;
            var skipCount = 0;
            var data = await _webClient.GetMetaphysicsList(skipCount: skipCount, maxCount: pageSize);
            if (data == null)
            {
                IsWebFailed = true;
                return null;
            }
            IsWebFailed = false;
            var maxCount = data.TotalCount;
            var tempData = new List<MetaPhysicsDto>(maxCount);
            tempData.AddRange(data.Items);

            while (tempData.Count < maxCount)
            {
                skipCount = tempData.Count;
                var newData = await _webClient.GetMetaphysicsList(skipCount: skipCount, maxCount: pageSize);
                if (newData == null || newData.Items.Count == 0) break;
                tempData.AddRange(newData.Items);
            }
            return tempData;
        }

        public override bool IsLocalOnly()
        {
            var result = _settings?.Ads.IsLocalOnly;
            return result.HasValue ? result.Value : false;
        }

        public override bool HasUpdate()
        {
            return _resourceUpdateProvider.ProductCommentsHasUpdate;
        }

        public override void TimeUpdated()
        {
            _resourceUpdateProvider.ProductCommentsUpdated();
        }
    }

    public class MetaApiCacheDataProvider
    {
        protected readonly IBizLogger logger = ServerLogFactory.GetLogger(typeof(ProductsApiCacheDataProvider));
        private SensingWebClient _webClient;
        private string _productBaseFolder;

        protected IResourceCacheManager _cacheManager;

        private string _resourceFolder = "Resources";
        private Settings _settings;
        private static object syncOjbect = new object();
        public int TotalDownloadCount { get; internal set; }
        public int DownloadedCount { get; internal set; }
        public int FailedCount { get; internal set; }

        private DownloadAlgorithm _algorithm;

        public MetaApiCacheDataProvider(SensingWebClient webClient, string productBaseFolder, Settings settings, DownloadAlgorithm algorithm = DownloadAlgorithm.WebClient) //mydownloader.
        {
            _webClient = webClient;
            _productBaseFolder = productBaseFolder;
            _algorithm = algorithm;
            _productJsonFullPath = Path.Combine(productBaseFolder, "Metas.json");
            _categoryJsonFullPath = Path.Combine(productBaseFolder, "DateMetas.json");
            _settings = settings;
        }
        public async Task Initialize()
        {
            try
            {
                if (_algorithm == DownloadAlgorithm.WebClient)
                {
                    _cacheManager = new WebClientLoalResourceCacheManager(_productBaseFolder, "res", true);
                }
                if (_algorithm == DownloadAlgorithm.Mydownload)
                {
                    _cacheManager = new LocalResourcesCacheManager(_productBaseFolder, "res", true);
                }
                _cacheManager.FileDownloaded += CacheManager_FileDownloaded;
                _cacheManager.AllDownloaded += CacheManager_AllDownloaded;
                ClearDownloadingFiles();
                await InitData();
                TotalDownloadCount = _cacheManager.GetDownloadFileCount();
            }
            catch (Exception ex)
            {
                logger.Error("Init Data Error", ex);
            }
        }

        public void StartDownload()
        {
            _cacheManager.StartDownloadResources();
        }

        private void ClearDownloadingFiles()
        {
            _cacheManager.RemoveDownloadFailedResources();
            var directory = new DirectoryInfo(FilePathHelper.CombinePath(_productBaseFolder, "res"));
            if (Directory.Exists(directory.FullName))
            {
                Parallel.ForEach(directory.GetFiles("*.downloading", SearchOption.AllDirectories), (path) => File.Delete(path.FullName));
            }
        }

        private void CacheManager_AllDownloaded(object sender, AllFilesDownloadedEventArgs arg)
        {
            OnDownloadedCompleted(new AllFilesDownloadedEventArgs());
        }
        public event FileDownloadedHandler FileDownloaded;
        public event AllFilesDownloadedHandler DownloadedCompleted;
        private void CacheManager_FileDownloaded(object sender, FileDownloadedEventArgs arg)
        {
            DownloadedCount++;
            if (arg.Status == DownloadedStatus.Failed)
            {
                FailedCount++;
            }
            Task.Run(() =>
            {
                OnFileDownloaded(arg);
            });
        }

        protected void OnFileDownloaded(FileDownloadedEventArgs args)
        {
            if (FileDownloaded != null)
            {
                FileDownloaded(this, args);
            }
        }

        protected void OnDownloadedCompleted(AllFilesDownloadedEventArgs args)
        {
            if (DownloadedCompleted != null)
            {
                DownloadedCompleted(this, args);
            }
        }

        /// <summary>
        /// Default is Cached
        /// </summary>
        private async Task InitData()
        {
            try
            {
                await AssembleProductData();
                await AssembleCategoryData();
            }
            catch (Exception e)
            {

            }
        }

        private void StartDownloads()
        {
            _cacheManager.StartDownloadResources();
        }

        #region Meta

        private List<MetaPhysicsDto> _webProductData = new List<MetaPhysicsDto>();
        private string _productJsonFullPath;
        private string _webProductsJsonData;
        public bool IsProductWebFailed { get; set; } = true;

        public List<MetaPhysicsDto> ProductsData
        {
            get { return _webProductData; }
            set { _webProductData = value; }
        }

        public async Task AssembleProductData()
        {
            //IsLocalOnly = True, if Local Data is miss,need to load from web.

            if (_settings.Products.IsLocalOnly)
            {
                if (ExistLocalData(_productJsonFullPath))
                {
                    ProductsData = ReadProductsDataFromLocal();
                }
                else
                {
                    ProductsData = await ReadProductsDataFromWeb();
                }
            }
            //Read from web first, then from local.
            else
            {
                ProductsData = await ReadProductsDataFromWeb();
                if (IsProductWebFailed)
                {
                    if (ExistLocalData(_productJsonFullPath))
                    {
                        ProductsData = ReadProductsDataFromLocal();
                    }
                }
            }

            IList<DownloadLink> newResources = ExtractProductLinks(SensingWebClient.MainServiceHost);
            if (!IsProductWebFailed)
            {
                WriteDataToJsonFile(ProductsData, _productJsonFullPath);
            }
            _cacheManager.AddResources(newResources);

        }

        private string AbsolutePath(string path, string baseUrl)
        {
            if (path.Contains("alicdn.com"))
            {
                return path;
            }
            if (!path.ToLower().StartsWith("http"))
            {
                var absolute = baseUrl + path;
                return absolute;//.ToLower();
            }
            return path;//.ToLower();
        }
        public virtual IList<DownloadLink> ExtractProductLinks(string baseUrl)
        {
            if (ProductsData == null) return null;

            baseUrl = SensingWebClient.RecommendApiHost;
            List<DownloadLink> links = new List<DownloadLink>();
            foreach (var item in ProductsData)
            {
                if (item.LogoUrl != null)
                {
                    if (!links.Any(link => link.RelativeFileName == item.LogoUrl))
                    {
                        links.Add(new DownloadLink { Host = baseUrl, RelativeFileName = item.LogoUrl, Type = "meta" });
                    }
                }
                if (item.PicUrl != null)
                {
                    if (!links.Any(link => link.RelativeFileName == item.PicUrl))
                    {
                        links.Add(new DownloadLink { Host = baseUrl, RelativeFileName = item.PicUrl, Type = "meta" });
                    }
                }
            }
            TotalDownloadCount += links.Count;
            logger.Debug($"Recommend Download Source Total is {TotalDownloadCount}");
            return links;
        }

        public async Task<List<MetaPhysicsDto>> ReadProductsDataFromWeb()
        {
            var pageSize = 300;
            var skipCount = 0;
            var data = await _webClient.GetMetaphysicsList(skipCount: skipCount, maxCount: pageSize);
            if (data == null)
            {
                IsProductWebFailed = true;
                return null;
            }
            IsProductWebFailed = false;
            var maxCount = data.TotalCount;
            var tempData = new List<MetaPhysicsDto>(maxCount);
            tempData.AddRange(data.Items);

            while (tempData.Count < maxCount)
            {
                skipCount = tempData.Count;
                var newData = await _webClient.GetMetaphysicsList(skipCount: skipCount, maxCount: pageSize);
                if (newData == null || newData.Items.Count == 0) break;
                tempData.AddRange(newData.Items);
            }
            return tempData;
        }
        public List<MetaPhysicsDto> ReadProductsDataFromLocal()
        {
            var jsonFile = _productJsonFullPath;
            if (File.Exists(jsonFile))
            {
                var jsondata = File.ReadAllText(jsonFile);
                return JsonConvert.DeserializeObject<List<MetaPhysicsDto>>(jsondata);
            }
            return null;
        }

        private List<MetaPhysicsDto> ReadDataFromJsonFile(string jsonFile)
        {
            try
            {
                if (File.Exists(jsonFile))
                {
                    return JsonConvert.DeserializeObject<List<MetaPhysicsDto>>(File.ReadAllText(jsonFile));
                }
            }
            catch (Exception e)
            {
                logger.Warning("Parse Json Faile!", e);
            }
            return null;
        }

        private List<ProductSdkModel> ReadDataFromJson(string jsonData)
        {
            try
            {
                return JsonConvert.DeserializeObject<List<ProductSdkModel>>(jsonData);
            }
            catch (Exception e)
            {
                logger.Warning("Parse Json Faile!", e);
            }
            return null;
        }

        #endregion

        #region DateMetas

        private List<DateMetaphysicsDto> _webCategoryData = new List<DateMetaphysicsDto>();
        private string _categoryJsonFullPath;
        private string _webCategoriesJsonData;
        private ResourceUpdateTimeProvider _resourceUpdateProvider;

        public bool IsCategoryWebFailed { get; set; } = true;

        public List<DateMetaphysicsDto> CategoriesData
        {
            get { return _webCategoryData; }
            set { _webCategoryData = value; }
        }

        public async Task<List<DateMetaphysicsDto>> ReadCategoriesDataFromWeb()
        {
            var endTime = DateTime.Now;
            //three week ago
            var startTime = endTime.AddDays(-60);
            var pageSize = 300;
            var skipCount = 0;
            var data = await _webClient.GetDateMetaPhysics(startTime, endTime, null, skipCount, pageSize);
            if (data == null)
            {
                IsCategoryWebFailed = true;
                return null;
            }
            IsCategoryWebFailed = false;
            var maxCount = data.TotalCount;
            var tempData = new List<DateMetaphysicsDto>(maxCount);
            tempData.AddRange(data.Items);

            while (tempData.Count < maxCount)
            {
                skipCount = tempData.Count;
                var newData = await _webClient.GetDateMetaPhysics(startTime, endTime, null, skipCount, pageSize);
                if (newData == null || newData.Items.Count == 0) break;
                tempData.AddRange(newData.Items);
            }
            return tempData;
        }

        public List<DateMetaphysicsDto> ReadCategoriesDataFromLocal()
        {
            var jsonFile = _categoryJsonFullPath;
            if (File.Exists(jsonFile))
            {
                var jsondata = File.ReadAllText(jsonFile);
                return JsonConvert.DeserializeObject<List<DateMetaphysicsDto>>(jsondata);
            }
            return null;
        }

        public virtual IList<DownloadLink> ExtractCategoryLinks(string baseUrl)
        {
            if (CategoriesData == null) return null;
            List<DownloadLink> links = new List<DownloadLink>();
            TotalDownloadCount += links.Count;
            logger.Debug($"Date Recommend Download Source Total is {links.Count}");
            return links;
        }
        public async Task AssembleCategoryData()
        {
            //IsLocalOnly = True, if Local Data is miss,need to load from web.
            if (_settings.Products.IsLocalOnly)
            {
                if (ExistLocalData(_categoryJsonFullPath))
                {
                    CategoriesData = ReadCategoriesDataFromLocal();
                }
                else
                {
                    CategoriesData = await ReadCategoriesDataFromWeb();
                }
            }
            //Read from web first, then from local.
            else
            {
                CategoriesData = await ReadCategoriesDataFromWeb();
                if (IsCategoryWebFailed)
                {
                    if (ExistLocalData(_categoryJsonFullPath))
                    {
                        CategoriesData = ReadCategoriesDataFromLocal();
                    }
                }
            }

            IList<DownloadLink> newResources = ExtractCategoryLinks(SensingWebClient.MainServiceHost);
            if (!IsCategoryWebFailed)
            {
                WriteDataToJsonFile(CategoriesData, _categoryJsonFullPath);
            }
            _cacheManager.AddResources(newResources);

        }

        #endregion

        private void WriteDataToJsonFile(object data, string filePath)
        {
            if (data != null)
            {
                var jsonData = JsonConvert.SerializeObject(data);
                if (!string.IsNullOrEmpty(jsonData))
                {
                    using (StreamWriter file = new StreamWriter(filePath, false))
                    {
                        file.WriteAsync(jsonData).Wait();
                    }
                }
            }
        }

        public bool ExistLocalData(string jsonPath)
        {
            return File.Exists(jsonPath);
        }
    }


    //public class DateRecommendApiCacheDataProvider : SingleCachedDataProviderBase<DateMetaphysicsDto>
    //{
    //    protected readonly IBizLogger logger = ServerLogFactory.GetLogger(typeof(DateRecommendApiCacheDataProvider));

    //    private SensingWebClient _webClient;
    //    private Settings _settings;
    //    public DateRecommendApiCacheDataProvider(SensingWebClient webClient, string metaBaseFolder, Settings settings, DownloadAlgorithm algorithm = DownloadAlgorithm.WebClient)
    //        : base(metaBaseFolder, "DateMetaPhysics.json", algorithm)
    //    {
    //        _webClient = webClient;
    //        _settings = settings;
    //    }

    //    public override IList<DownloadLink> ExtractLinks(string baseUrl)
    //    {
    //        return null;
    //    }

    //    public override async Task<List<DateMetaphysicsDto>> ReadDataFromWeb()
    //    {
    //        var now = DateTime.Now;
    //        var pageSize = 300;
    //        var skipCount = 0;
    //        var data = await _webClient.GetDateMetaPhysics(now, now.AddDays(7),skipCount, pageSize);
    //        if (data == null)
    //        {
    //            IsWebFailed = true;
    //            return null;
    //        }
    //        IsWebFailed = false;
    //        var maxCount = data.TotalCount;
    //        var tempData = new List<DateMetaphysicsDto>(maxCount);
    //        tempData.AddRange(data.Items);

    //        while (tempData.Count < maxCount)
    //        {
    //            skipCount = tempData.Count;
    //            var newData = await _webClient.GetDateMetaPhysics(now, now.AddDays(7), skipCount, pageSize);
    //            if (newData == null || newData.Items.Count == 0) break;
    //            tempData.AddRange(newData.Items);
    //        }
    //        return tempData;
    //    }

    //    public override bool IsLocalOnly()
    //    {
    //        var result = _settings?.Ads.IsLocalOnly;
    //        return result.HasValue ? result.Value : false;
    //    }
    //}
}