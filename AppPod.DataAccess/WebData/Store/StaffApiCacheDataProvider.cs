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
    public class StaffApiCacheDataProvider
    {
        protected readonly IBizLogger logger = ServerLogFactory.GetLogger(typeof(ProductsApiCacheDataProvider));

        private SensingWebClient _webClient;
        private string _staffBaseFolder;

        protected IResourceCacheManager _cacheManager;

        private List<StaffSdkModel> _webProductData = new List<StaffSdkModel>();
        private string _staffJsonFullPath;
        private string _webProductsJsonData;
        public bool IsStaffWebFailed { get; set; } = true;


        private string _resourceFolder = "Resources";

        private Settings _settings;

        private static object syncOjbect = new object();
        public int TotalDownloadCount { get; internal set; }
        public int DownloadedCount { get; internal set; }

        public int FailedCount { get; internal set; }

        private DownloadAlgorithm _algorithm;
        private ResourceUpdateTimeProvider _resourceUpdateProvider;

        public StaffApiCacheDataProvider(SensingWebClient webClient, string productBaseFolder, Settings settings, DownloadAlgorithm algorithm = DownloadAlgorithm.WebClient) //mydownloader.
        {
            _webClient = webClient;
            _staffBaseFolder = productBaseFolder;
            _algorithm = algorithm;
            _staffJsonFullPath = Path.Combine(productBaseFolder, "Staffs.json");
            _settings = settings;
        }

        public List<StaffSdkModel> StaffsData
        {
            get { return _webProductData; }
            set { _webProductData = value; }
        }



        public async Task Initialize(ResourceUpdateTimeProvider resourceUpdateProvider)
        {
            try
            {
                _resourceUpdateProvider = resourceUpdateProvider;
                if (_algorithm == DownloadAlgorithm.WebClient)
                {
                    _cacheManager = new WebClientLoalResourceCacheManager(_staffBaseFolder, "res", true);
                }
                if (_algorithm == DownloadAlgorithm.Mydownload)
                {
                    _cacheManager = new LocalResourcesCacheManager(_staffBaseFolder, "res", true);
                }
                _cacheManager.FileDownloaded += CacheManager_FileDownloaded;
                _cacheManager.AllDownloaded += CacheManager_AllDownloaded;
                ClearDownloadingFiles();
                await InitData();
            }
            catch (Exception ex)
            {
                logger.Error("Init Data Error", ex);
            }
        }

        private void ClearDownloadingFiles()
        {
            _cacheManager.RemoveDownloadFailedResources();
            var directory = new DirectoryInfo(FilePathHelper.CombinePath(_staffBaseFolder, "res"));
            if (Directory.Exists(directory.FullName))
                Parallel.ForEach(directory.GetFiles("*.downloading", SearchOption.AllDirectories), (path) => File.Delete(path.FullName));
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
            if (FileDownloaded != null)
            {
                DownloadedCompleted(this, args);
            }
        }
        /// <summary>
        /// Default is Cached
        /// </summary>
        private async Task InitData()
        {
            await AssembleProductData();
            TotalDownloadCount = _cacheManager.GetDownloadFileCount();
        }

        public void StartDownload()
        {
            _cacheManager.StartDownloadResources();
        }

        public bool ExistLocalData()
        {
            return File.Exists(_staffJsonFullPath);
        }
        public async Task AssembleProductData()
        {
            //IsLocalOnly = True, if Local Data is miss,need to load from web.
            if (_settings.Ads.IsLocalOnly || !_resourceUpdateProvider.StaffsHasUpdate)
            {
                if (ExistLocalData())
                {
                    StaffsData = ReadProductsDataFromLocal();
                }
                else
                {
                    StaffsData = await ReadProductsDataFromWeb();
                }
            }
            //Read from web first, then from local.
            else
            {
                StaffsData = await ReadProductsDataFromWeb();
                if (IsStaffWebFailed)
                {
                    if (ExistLocalData())
                    {
                        StaffsData = ReadProductsDataFromLocal();
                    }
                }
            }

            IList<DownloadLink> newResources = ExtractStaffLinks(SensingWebClient.MainServiceHost);
            if (!IsStaffWebFailed)
            {
                WriteDataToJsonFile(StaffsData, _staffJsonFullPath);
                _resourceUpdateProvider.StaffsUpdated();
            }
            _cacheManager.AddResources(newResources);
        }

        private void StartDownloads()
        {
            _cacheManager.StartDownloadResources();
        }
        public virtual IList<DownloadLink> ExtractStaffLinks(string baseUrl)
        {
            if (StaffsData == null) return null;
            List<DownloadLink> links = new List<DownloadLink>();
            foreach (var item in StaffsData)
            {
                //if (item.AvatarUrl != null)
                //{
                //    if (!links.Any(link => link.RelativeFileName == item.AvatarUrl))
                //    {
                //        links.Add(new DownloadLink { Host = baseUrl, RelativeFileName = item.AvatarUrl, Type = "staff" });
                //    }
                //}
            }
            TotalDownloadCount = links.Count;
            logger.Debug($"Staffs Download Source Total is {TotalDownloadCount}");
            return links;
        }

        public async Task<List<StaffSdkModel>> ReadProductsDataFromWeb()
        {
            var pageSize = 300;
            var skipCount = 1;
            var data = await _webClient.GetStaffs(skipCount, pageSize);
            if (data == null)
            {
                IsStaffWebFailed = true;
                return null;
            }
            IsStaffWebFailed = false;
            var maxCount = data.TotalCount;
            var tempData = new List<StaffSdkModel>(maxCount);
            tempData.AddRange(data.Items);
            while (tempData.Count < maxCount)
            {

                var newData = await _webClient.GetStaffs(tempData.Count, pageSize);
                if (newData == null) break;
                tempData.AddRange(newData.Items);
            }

            _webProductsJsonData = JsonConvert.SerializeObject(tempData);
            return tempData;
        }

        public List<StaffSdkModel> ReadProductsDataFromLocal()
        {
            var jsonFile = _staffJsonFullPath;
            if (File.Exists(jsonFile))
            {
                var jsondata = File.ReadAllText(jsonFile);
                return JsonConvert.DeserializeObject<List<StaffSdkModel>>(jsondata);
            }
            return null;
        }
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

    }
}
