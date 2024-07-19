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
using System.Xml.Linq;

namespace AppPod.DataAccess.WebData.Store
{
    public enum DownloadAlgorithm
    {
        Mydownload,
        WebClient
    }
    public class ProductsApiCacheDataProvider
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

        public ProductsApiCacheDataProvider(SensingWebClient webClient, string productBaseFolder, Settings settings, DownloadAlgorithm algorithm = DownloadAlgorithm.WebClient) //mydownloader.
        {
            _webClient = webClient;
            _productBaseFolder = productBaseFolder;
            _algorithm = algorithm;
            _productJsonFullPath = Path.Combine(productBaseFolder, "Products.json");
            _categoryJsonFullPath = Path.Combine(productBaseFolder, "Categories.json");
            _tagsJsonFullPath = Path.Combine(productBaseFolder, "ProductTags.json");
            _deviceTagsJsonFullPath = Path.Combine(productBaseFolder, "DeviceTags.json");
            _matchJsonFullPath = Path.Combine(productBaseFolder, "Matches.json");
            _likeJsonFullPath = Path.Combine(productBaseFolder, "Likes.json");
            _couponJsonFullPath = Path.Combine(productBaseFolder, "Coupons.json");
            _propertiesJsonFullPath = Path.Combine(productBaseFolder, "Properties.json");
            _commentsJsonFullPath = Path.Combine(productBaseFolder, "ProductComments.json");
            _brandJsonFullPath = Path.Combine(productBaseFolder, "Brands.json");
            _activityGamesJsonFullPath = Path.Combine(productBaseFolder, "ActivityGames.json");
            _settings = settings;
        }
        public async Task Initialize(ResourceUpdateTimeProvider resourceUpdateProvider)
        {
            try
            {
                _resourceUpdateProvider = resourceUpdateProvider;
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
                await AssembleBrandsData();
                await AssembleProductData();
                await AssembleCategoryData();
                await AssembleTagsData();
                await AssembleDeviceTagsData();
                await AssembleMatchData();
                await AssembleLikeData();
                await AssembleCoponData();
                await AssemblePropertiesData();
                await AssembleCommentsData();
                await AssembleActivityGamesData();
                _resourceUpdateProvider.Save();
            }
            catch (Exception e)
            {

            }
        }

        private void StartDownloads()
        {
            _cacheManager.StartDownloadResources();
        }

        #region Coupon

        private List<CouponViewModel> _webCouponData = new List<CouponViewModel>();
        private string _couponJsonFullPath;
        private string _webCouponJsonData;
        public bool IsCouponWebFailed { get; set; } = true;

        public List<CouponViewModel> CouponData
        {
            get { return _webCouponData; }
            set { _webCouponData = value; }
        }

        private async Task AssembleCoponData()
        {
            //IsLocalOnly = True, if Local Data is miss,need to load from web.
            if (_settings.Products.IsLocalOnly || !_resourceUpdateProvider.CouponsHasUpdate)
            {
                if (ExistLocalData(_couponJsonFullPath))
                {
                    CouponData = ReadCouponDataFromLocal();
                }
                else
                {
                    CouponData = await ReadCouponDataFromWeb();
                }
            }
            //Read from web first, then from local.
            else
            {
                CouponData = await ReadCouponDataFromWeb();
                if (IsCouponWebFailed)
                {
                    if (ExistLocalData(_couponJsonFullPath))
                    {
                        CouponData = ReadCouponDataFromLocal();
                    }
                }
            }

            IList<DownloadLink> newResources = ExtractCoponsLinks(SensingWebClient.MainServiceHost);
            if (!IsCouponWebFailed)
            {
                WriteDataToJsonFile(CouponData, _couponJsonFullPath);
                _resourceUpdateProvider.CouponsUpdated();
            }
            _cacheManager.AddResources(newResources);
        }

        private async Task<List<CouponViewModel>> ReadCouponDataFromWeb()
        {

            var skipCount = 0;
            var data = await _webClient.GetCoupons(skipCount, 500);
            if (data == null)
            {
                IsCouponWebFailed = true;
                return null;
            }
            IsCouponWebFailed = false;
            var tempData = new List<CouponViewModel>(data.TotalCount);
            if (data.TotalCount <= 500)
            {
                tempData.AddRange(data.Items);
            }

            _webCouponJsonData = JsonConvert.SerializeObject(tempData);
            return tempData;
        }
        private IList<DownloadLink> ExtractCoponsLinks(string serverBase)
        {
            if (CouponData == null) return null;
            List<DownloadLink> links = new List<DownloadLink>();
            foreach (var item in CouponData)
            {
                if (item.Pictures != null)
                {
                    if (!links.Any(link => link.RelativeFileName == item.Pictures))
                    {
                        links.Add(new DownloadLink { Host = serverBase, RelativeFileName = item.Pictures, Type = "Coupon" });
                    }
                }
            }
            TotalDownloadCount += links.Count;
            logger.Debug($"Match Download Source Total is {TotalDownloadCount}");
            return links;
        }

        private List<CouponViewModel> ReadCouponDataFromLocal()
        {
            var jsonFile = _couponJsonFullPath;
            if (File.Exists(jsonFile))
            {
                var jsondata = File.ReadAllText(jsonFile);
                return JsonConvert.DeserializeObject<List<CouponViewModel>>(jsondata);
            }
            return null;
        }

        #endregion

        #region Likes

        private List<LikeInfoViewModel> _webLikeData = new List<LikeInfoViewModel>();
        private string _likeJsonFullPath;
        private string _webLikeJsonData;
        public bool IsLikeWebFailed { get; set; } = true;

        public List<LikeInfoViewModel> LikesData
        {
            get { return _webLikeData; }
            set { _webLikeData = value; }
        }
        private async Task AssembleLikeData()
        {
            //IsLocalOnly = True, if Local Data is miss,need to load from web.
            if (_settings.Products.IsLocalOnly || !_resourceUpdateProvider.LikeInfosHasUpdate)
            {
                if (ExistLocalData(_likeJsonFullPath))
                {
                    LikesData = ReadLikeDataFromLocal();
                }
                else
                {
                    LikesData = await ReadLikeDataFromWeb();
                }
            }
            //Read from web first, then from local.
            else
            {
                LikesData = await ReadLikeDataFromWeb();
                if (IsLikeWebFailed)
                {
                    if (ExistLocalData(_likeJsonFullPath))
                    {
                        LikesData = ReadLikeDataFromLocal();
                    }
                }
            }

            IList<DownloadLink> newResources = ExtractLikeLinks(SensingWebClient.MainServiceHost);
            if (!IsLikeWebFailed)
            {
                _resourceUpdateProvider.LikeInfosUpdated();
                WriteDataToJsonFile(LikesData, _likeJsonFullPath);
            }
            _cacheManager.AddResources(newResources);
        }

        private IList<DownloadLink> ExtractLikeLinks(string serverBase)
        {
            return null;
        }



        private async Task<List<LikeInfoViewModel>> ReadLikeDataFromWeb()
        {

            var data = await _webClient.GetLikeInfos(1, 500);
            if (data == null)
            {
                IsLikeWebFailed = true;
                return null;
            }
            IsLikeWebFailed = false;
            var tempData = new List<LikeInfoViewModel>(data.TotalCount);
            if (data.TotalCount <= 500)
            {
                tempData.AddRange(data.Items);
            }

            _webLikeJsonData = JsonConvert.SerializeObject(tempData);
            return tempData;
        }

        private List<LikeInfoViewModel> ReadLikeDataFromLocal()
        {
            var jsonFile = _likeJsonFullPath;
            if (File.Exists(jsonFile))
            {
                var jsondata = File.ReadAllText(jsonFile);
                return JsonConvert.DeserializeObject<List<LikeInfoViewModel>>(jsondata);
            }
            return null;
        }

        #endregion

        #region Match

        private List<MatchInfoViewModel> _webMatchData = new List<MatchInfoViewModel>();
        private string _matchJsonFullPath;
        private string _webMatchJsonData;
        public bool IsMatchWebFailed { get; set; } = true;

        public List<MatchInfoViewModel> MatchesData
        {
            get { return _webMatchData; }
            set { _webMatchData = value; }
        }

        private async Task AssembleMatchData()
        {
            //IsLocalOnly = True, if Local Data is miss,need to load from web.
            if (_settings.Products.IsLocalOnly || !_resourceUpdateProvider.MatchInfosHasUpdate)
            {
                if (ExistLocalData(_likeJsonFullPath))
                {
                    MatchesData = ReadMatchDataFromLocal();
                }
                else
                {
                    MatchesData = await ReadMatchDataFromWeb();
                }
            }
            //Read from web first, then from local.
            else
            {
                MatchesData = await ReadMatchDataFromWeb();
                if (IsMatchWebFailed)
                {
                    if (ExistLocalData(_matchJsonFullPath))
                    {
                        MatchesData = ReadMatchDataFromLocal();
                    }
                }
            }

            IList<DownloadLink> newResources = ExtractMatchLinks(SensingWebClient.MainServiceHost);
            if (!IsMatchWebFailed)
            {
                WriteDataToJsonFile(MatchesData, _matchJsonFullPath);
                _resourceUpdateProvider.MatchInfosUpdated();
            }
            _cacheManager.AddResources(newResources);
        }

        private async Task<List<MatchInfoViewModel>> ReadMatchDataFromWeb()
        {

            var data = await _webClient.GetMatchInfos(1, 500);
            if (data == null)
            {
                IsMatchWebFailed = true;
                return null;
            }
            IsMatchWebFailed = false;
            var tempData = new List<MatchInfoViewModel>(data.TotalCount);
            if (data.TotalCount <= 500)
            {
                tempData.AddRange(data.Items);
            }

            _webMatchJsonData = JsonConvert.SerializeObject(tempData);
            return tempData;
        }

        private List<MatchInfoViewModel> ReadMatchDataFromLocal()
        {
            var jsonFile = _matchJsonFullPath;
            if (File.Exists(jsonFile))
            {
                var jsondata = File.ReadAllText(jsonFile);
                return JsonConvert.DeserializeObject<List<MatchInfoViewModel>>(jsondata);
            }
            return null;
        }

        private IList<DownloadLink> ExtractMatchLinks(string baseUrl)
        {
            if (MatchesData == null) return null;
            List<DownloadLink> links = new List<DownloadLink>();
            foreach (var item in MatchesData)
            {
                if (item.ShowImage != null)
                {
                    if (!links.Any(link => link.RelativeFileName == item.ShowImage))
                    {
                        links.Add(new DownloadLink { Host = baseUrl, RelativeFileName = item.ShowImage, Type = "Match" });
                    }
                }
                //SKU的图已经在SKU里面
                //var skus = item.MatchItems;
                //if (skus != null)
                //{
                //    foreach (var sku in skus)
                //    {
                //        if (sku != null)
                //        {
                //            if (!links.Any(link => link.RelativeFileName == sku.SkuPicUrl))
                //            {
                //                links.Add(new DownloadLink { Host = baseUrl, RelativeFileName = sku.SkuPicUrl, Type = "product" });
                //            }
                //        }
                //    }
                //}
            }
            TotalDownloadCount += links.Count;
            logger.Debug($"Match Download Source Total is {TotalDownloadCount}");
            return links;
        }

        #endregion

        #region Products

        private List<ProductSdkModel> _webProductData = new List<ProductSdkModel>();
        private string _productJsonFullPath;
        private string _webProductsJsonData;
        public bool IsProductWebFailed { get; set; } = true;

        public List<ProductSdkModel> ProductsData
        {
            get { return _webProductData; }
            set { _webProductData = value; }
        }

        public async Task AssembleProductData()
        {
            //IsLocalOnly = True, if Local Data is miss,need to load from web.

            if (_settings.Products.IsLocalOnly || !_resourceUpdateProvider.ProductsSkuPropertyValuesHasUpdate)
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
                _resourceUpdateProvider.ProductsSkuPropertyValuesUpdated();
            }
            _cacheManager.AddResources(newResources);

        }

        private string AbsolutePath(string path, string baseUrl)
        {
            if (path == null)
            {
                return path;
            }
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
            List<DownloadLink> links = new List<DownloadLink>();
            foreach (var item in ProductsData)
            {
                if (item.PicUrl != null)
                {
                    var absolute = AbsolutePath(item.PicUrl, baseUrl);

                    if (!links.Any(link => link.RelativeFileName == absolute))
                    {
                        links.Add(new DownloadLink { Host = baseUrl, RelativeFileName = absolute, Type = "product" });
                    }
                }
                if (item.ItemImagesOrVideos != null)
                {
                    foreach (var res in item.ItemImagesOrVideos)
                    {
                        var absolute = AbsolutePath(res.FileUrl, baseUrl);
                        if (!links.Any(link => link.RelativeFileName == absolute))
                        {
                            links.Add(new DownloadLink { Host = baseUrl, RelativeFileName = absolute, Type = "product" });
                        }
                    }
                }

                if (item.PropImgs != null)
                {
                    foreach (var pImg in item.PropImgs)
                    {
                        var absolute = AbsolutePath(pImg.ImageUrl, baseUrl);
                        if (!links.Any(link => link.RelativeFileName == absolute))
                        {
                            links.Add(new DownloadLink { Host = baseUrl, RelativeFileName = absolute, Type = "product" });
                        }
                    }
                }
                var skus = item.Skus;
                if (skus != null)
                {
                    foreach (var sku in skus)
                    {
                        if (sku != null)
                        {
                            if (sku.ItemImagesOrVideos != null)
                            {
                                foreach (var res in sku.ItemImagesOrVideos)
                                {
                                    var absolute = AbsolutePath(res.FileUrl, baseUrl);
                                    if (!links.Any(link => link.RelativeFileName == absolute))
                                    {
                                        links.Add(new DownloadLink { Host = baseUrl, RelativeFileName = absolute, Type = "sku" });
                                    }
                                }
                            }
                            if (sku.OnlineStoreInfos != null)
                            {
                                foreach (var res in sku.OnlineStoreInfos)
                                {
                                    if (string.IsNullOrEmpty(res?.Qrcode))
                                        continue;
                                    if (res.Qrcode.EndsWith(".png") || res.Qrcode.EndsWith(".jpg"))
                                    {
                                        var absolute = AbsolutePath(res.Qrcode, baseUrl);
                                        if (!links.Any(link => link.RelativeFileName == absolute))
                                        {
                                            links.Add(new DownloadLink { Host = baseUrl, RelativeFileName = absolute, Type = "product" });
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            TotalDownloadCount += links.Count;
            logger.Debug($"Product Download Source Total is {TotalDownloadCount}");
            return links;
        }

        public async Task<List<ProductSdkModel>> ReadProductsDataFromWeb()
        {

            var pageSize = 100;
            var skipCount = 0;

            var tempData = new List<ProductSdkModel>();
            var data = await _webClient.GetProducts(skipCount, pageSize);
            if (data == null)
            {
                IsProductWebFailed = true;
                return null;
            }
            IsProductWebFailed = false;
            var maxCount = data.TotalCount;
            tempData = new List<ProductSdkModel>(maxCount);
            tempData.AddRange(data.Items);

            while (tempData.Count < maxCount)
            {
                skipCount = tempData.Count;
                var newData = await _webClient.GetProducts(skipCount, pageSize);
                if (newData == null) break;
                tempData.AddRange(newData.Items);
            }
            _webProductsJsonData = JsonConvert.SerializeObject(tempData);
            return tempData;
        }

        public List<ProductSdkModel> ReadProductsDataFromLocal()
        {
            var jsonFile = _productJsonFullPath;
            if (File.Exists(jsonFile))
            {
                var jsondata = File.ReadAllText(jsonFile);
                return JsonConvert.DeserializeObject<List<ProductSdkModel>>(jsondata);
            }
            return null;
        }

        private List<ProductSdkModel> ReadDataFromJsonFile(string jsonFile)
        {
            try
            {
                if (File.Exists(jsonFile))
                {
                    return JsonConvert.DeserializeObject<List<ProductSdkModel>>(File.ReadAllText(jsonFile));
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

        #region Categories

        private List<ProductCategorySDKModel> _webCategoryData = new List<ProductCategorySDKModel>();
        private string _categoryJsonFullPath;
        private string _webCategoriesJsonData;
        public bool IsCategoryWebFailed { get; set; } = true;

        public List<ProductCategorySDKModel> CategoriesData
        {
            get { return _webCategoryData; }
            set { _webCategoryData = value; }
        }

        public async Task<List<ProductCategorySDKModel>> ReadCategoriesDataFromWeb()
        {
            var tempData = new List<ProductCategorySDKModel>();
            var data = await _webClient.GetProductCategories();
            if (data == null)
            {
                IsCategoryWebFailed = true;
                return null;
            }
            IsCategoryWebFailed = false;

            tempData.AddRange(data.Items);
            _webCategoriesJsonData = JsonConvert.SerializeObject(tempData);
            return tempData;
        }

        public List<ProductCategorySDKModel> ReadCategoriesDataFromLocal()
        {
            var jsonFile = _categoryJsonFullPath;
            if (File.Exists(jsonFile))
            {
                var jsondata = File.ReadAllText(jsonFile);
                return JsonConvert.DeserializeObject<List<ProductCategorySDKModel>>(jsondata);
            }
            return null;
        }

        public virtual IList<DownloadLink> ExtractCategoryLinks(string baseUrl)
        {
            if (CategoriesData == null) return null;
            List<DownloadLink> links = new List<DownloadLink>();
            foreach (var item in CategoriesData)
            {
                if (!string.IsNullOrEmpty(item.IconUrl))
                {
                    if (!links.Any(link => link.RelativeFileName == item.IconUrl))
                    {
                        links.Add(new DownloadLink { Host = baseUrl, RelativeFileName = item.IconUrl, Type = "Category" });
                    }
                }

                if (!string.IsNullOrEmpty(item.ImageUrl))
                {
                    if (!links.Any(link => link.RelativeFileName == item.ImageUrl))
                    {
                        links.Add(new DownloadLink { Host = baseUrl, RelativeFileName = item.ImageUrl, Type = "Category" });
                    }
                }
            }
            TotalDownloadCount += links.Count;
            logger.Debug($"Categories Download Source Total is {links.Count}");
            return links;
        }
        public async Task AssembleCategoryData()
        {
            //IsLocalOnly = True, if Local Data is miss,need to load from web.
            if (_settings.Products.IsLocalOnly || !_resourceUpdateProvider.ProductCategoriesHasUpdate)
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
                _resourceUpdateProvider.ProductCategoriesUpdated();
            }
            _cacheManager.AddResources(newResources);

        }

        #endregion

        #region Tags

        private List<TagSdkModel> _webTagData = new List<TagSdkModel>();
        private string _tagsJsonFullPath;
        private string _webTagsJsonData;
        public bool IsTagsWebFailed { get; set; } = true;

        private List<TagSdkModel> _webDeviceTagData = new List<TagSdkModel>();
        private string _deviceTagsJsonFullPath;
        private string _webDeviceTagsJsonData;
        public bool IsDeviceTagsWebFailed { get; set; } = true;

        public List<TagSdkModel> TagsData
        {
            get { return _webTagData; }
            set { _webTagData = value; }
        }

        public List<TagSdkModel> DeviceTagsData
        {
            get { return _webDeviceTagData; }
            set { _webDeviceTagData = value; }
        }

        public async Task<List<TagSdkModel>> ReadTagsDataFromWeb()
        {
            var tempData = new List<TagSdkModel>();
            var data = await _webClient.GetProductTags();
            if (data == null)
            {
                IsTagsWebFailed = true;
                return null;
            }
            IsTagsWebFailed = false;

            tempData.AddRange(data.Items);
            _webTagsJsonData = JsonConvert.SerializeObject(tempData);
            return tempData;
        }


        public async Task<List<TagSdkModel>> ReadDeviceTagsDataFromWeb()
        {
            var tempData = new List<TagSdkModel>();
            var data = await _webClient.GetDeviceTags();
            if (data == null)
            {
                IsDeviceTagsWebFailed = true;
                return null;
            }
            IsDeviceTagsWebFailed = false;

            tempData.AddRange(data.Items);
            _webDeviceTagsJsonData = JsonConvert.SerializeObject(tempData);
            return tempData;
        }

        public List<TagSdkModel> ReadTagsDataFromLocal()
        {
            var jsonFile = _tagsJsonFullPath;
            if (File.Exists(jsonFile))
            {
                var jsondata = File.ReadAllText(jsonFile);
                return JsonConvert.DeserializeObject<List<TagSdkModel>>(jsondata);
            }
            return null;
        }

        public List<TagSdkModel> ReadDeviceTagsDataFromLocal()
        {
            var jsonFile = _deviceTagsJsonFullPath;
            if (File.Exists(jsonFile))
            {
                var jsondata = File.ReadAllText(jsonFile);
                return JsonConvert.DeserializeObject<List<TagSdkModel>>(jsondata);
            }
            return null;
        }

        public virtual IList<DownloadLink> ExtractTagsLinks(string baseUrl)
        {
            if (TagsData == null) return null;
            List<DownloadLink> links = new List<DownloadLink>();
            foreach (var item in TagsData)
            {
                if (item.IconUrl != null)
                {
                    if (!links.Any(link => link.RelativeFileName == item.IconUrl))
                    {
                        links.Add(new DownloadLink { Host = baseUrl, RelativeFileName = item.IconUrl, Type = "Category" });
                    }
                }
            }
            TotalDownloadCount += links.Count;
            logger.Debug($"Categories Download Source Total is {links.Count}");
            return links;
        }


        public virtual IList<DownloadLink> ExtractDeviceTagsLinks(string baseUrl)
        {
            if (DeviceTagsData == null) return null;
            List<DownloadLink> links = new List<DownloadLink>();
            foreach (var item in DeviceTagsData)
            {
                if (item.IconUrl != null)
                {
                    if (!links.Any(link => link.RelativeFileName == item.IconUrl))
                    {
                        links.Add(new DownloadLink { Host = baseUrl, RelativeFileName = item.IconUrl, Type = "Category" });
                    }
                }
            }
            TotalDownloadCount += links.Count;
            logger.Debug($"Categories Download Source Total is {links.Count}");
            return links;
        }

        public async Task AssembleTagsData()
        {
            //IsLocalOnly = True, if Local Data is miss,need to load from web.
            if (_settings.Products.IsLocalOnly || !_resourceUpdateProvider.TagsHasUpdate)
            {
                if (ExistLocalData(_tagsJsonFullPath))
                {
                    TagsData = ReadTagsDataFromLocal();
                }
                else
                {
                    TagsData = await ReadTagsDataFromWeb();
                }
            }
            //Read from web first, then from local.
            else
            {
                TagsData = await ReadTagsDataFromWeb();
                if (IsTagsWebFailed)
                {
                    if (ExistLocalData(_tagsJsonFullPath))
                    {
                        TagsData = ReadTagsDataFromLocal();
                    }
                }
            }

            IList<DownloadLink> newResources = ExtractTagsLinks(SensingWebClient.MainServiceHost);
            _cacheManager.AddResources(newResources);
            if (!IsTagsWebFailed)
            {
                WriteDataToJsonFile(TagsData, _tagsJsonFullPath);
                _resourceUpdateProvider.TagsUpdated();
            }


        }


        public async Task AssembleDeviceTagsData()
        {
            //IsLocalOnly = True, if Local Data is miss,need to load from web.
            if (_settings.Products.IsLocalOnly || !_resourceUpdateProvider.TagsHasUpdate)
            {
                if (ExistLocalData(_deviceTagsJsonFullPath))
                {
                    DeviceTagsData = ReadDeviceTagsDataFromLocal();
                }
                else
                {
                    DeviceTagsData = await ReadDeviceTagsDataFromWeb();
                }
            }
            //Read from web first, then from local.
            else
            {
                DeviceTagsData = await ReadDeviceTagsDataFromWeb();
                if (IsDeviceTagsWebFailed)
                {
                    if (ExistLocalData(_deviceTagsJsonFullPath))
                    {
                        DeviceTagsData = ReadDeviceTagsDataFromLocal();
                    }
                }
            }

            IList<DownloadLink> newResources = ExtractTagsLinks(SensingWebClient.MainServiceHost);
            _cacheManager.AddResources(newResources);
            if (!IsDeviceTagsWebFailed)
            {
                WriteDataToJsonFile(DeviceTagsData, _deviceTagsJsonFullPath);
                _resourceUpdateProvider.TagsUpdated();
            }
        }

        #endregion

        #region ProductComments

        private List<ProductCommentModel> _webCommentsData = new List<ProductCommentModel>();
        private string _commentsJsonFullPath;
        private string _webCommentsJsonData;
        public bool IsCommentsWebFailed { get; set; } = true;

        public List<ProductCommentModel> ProductComments
        {
            get { return _webCommentsData; }
            set { _webCommentsData = value; }
        }

        public async Task<List<ProductCommentModel>> ReadCommentssDataFromWeb()
        {
            var tempData = new List<ProductCommentModel>();
            var data = await _webClient.GetProductComments();
            if (data == null)
            {
                IsCommentsWebFailed = true;
                return null;
            }
            IsCommentsWebFailed = false;

            tempData.AddRange(data.Items);
            _webCommentsJsonData = JsonConvert.SerializeObject(tempData);
            return tempData;
        }

        public List<ProductCommentModel> ReadCommentsDataFromLocal()
        {
            var jsonFile = _commentsJsonFullPath;
            if (File.Exists(jsonFile))
            {
                var jsondata = File.ReadAllText(jsonFile);
                return JsonConvert.DeserializeObject<List<ProductCommentModel>>(jsondata);
            }
            return null;
        }

        public virtual IList<DownloadLink> ExtractCommentsLinks(string baseUrl)
        {
            if (ProductComments == null) return null;
            List<DownloadLink> links = new List<DownloadLink>();

            logger.Debug($"Categories Download Source Total is {links.Count}");
            return links;
        }
        public async Task AssembleCommentsData()
        {
            //IsLocalOnly = True, if Local Data is miss,need to load from web.
            logger.Debug("AssembleCommentsData");
            if (_settings.Products.IsLocalOnly || !_resourceUpdateProvider.ProductCommentsHasUpdate)
            {
                logger.Debug(_commentsJsonFullPath);
                if (ExistLocalData(_commentsJsonFullPath))
                {
                    ProductComments = ReadCommentsDataFromLocal();
                }
                else
                {
                    ProductComments = await ReadCommentssDataFromWeb();
                }
            }
            //Read from web first, then from local.
            else
            {
                logger.Debug("ReadCommentssDataFromWeb");
                ProductComments = await ReadCommentssDataFromWeb();
                logger.Debug("ReadCommentssDataFromWeb Count" + ProductComments?.Count);
                if (IsCommentsWebFailed)
                {
                    if (ExistLocalData(_commentsJsonFullPath))
                    {
                        ProductComments = ReadCommentsDataFromLocal();
                    }
                }
            }

            if (!IsCommentsWebFailed)
            {
                WriteDataToJsonFile(ProductComments, _commentsJsonFullPath);
                _resourceUpdateProvider.ProductCommentsUpdated();
            }

        }

        #endregion


        #region ProductBrands

        private List<BrandDto> _webBrandsData = new List<BrandDto>();
        private string _brandJsonFullPath;
        private string _webBrandJsonData;
        public bool IsBrandWebFailed { get; set; } = true;

        public List<BrandDto> Brands
        {
            get { return _webBrandsData; }
            set { _webBrandsData = value; }
        }

        public async Task<List<BrandDto>> ReadBrandsDataFromWeb()
        {
            var tempData = new List<BrandDto>();
            var data = await _webClient.GetBrands();
            if (data == null)
            {
                IsBrandWebFailed = true;
                return null;
            }
            IsBrandWebFailed = false;

            tempData.AddRange(data.Items);
            _webBrandJsonData = JsonConvert.SerializeObject(tempData);
            return tempData;
        }

        public List<BrandDto> ReadBrandsDataFromLocal()
        {
            var jsonFile = _brandJsonFullPath;
            if (File.Exists(jsonFile))
            {
                var jsondata = File.ReadAllText(jsonFile);
                return JsonConvert.DeserializeObject<List<BrandDto>>(jsondata);
            }
            return null;
        }

        public virtual IList<DownloadLink> ExtractBrandsLinks(string baseUrl)
        {
            if (Brands == null) return null;
            List<DownloadLink> links = new List<DownloadLink>();
            foreach (var item in Brands)
            {
                if (item.LogoUrl != null)
                {
                    if (!links.Any(link => link.RelativeFileName == item.LogoUrl))
                    {
                        links.Add(new DownloadLink { Host = baseUrl, RelativeFileName = item.LogoUrl, Type = "Brands" });
                    }
                }
                if (item.ImageUrl != null)
                {
                    if (!links.Any(link => link.RelativeFileName == item.ImageUrl))
                    {
                        links.Add(new DownloadLink { Host = baseUrl, RelativeFileName = item.ImageUrl, Type = "Brands" });
                    }
                }
                if (item.ItemImagesOrVideos != null)
                {
                    foreach (var source in item.ItemImagesOrVideos)
                    {
                        if (!links.Any(link => link.RelativeFileName == source.FileUrl))
                        {
                            links.Add(new DownloadLink { Host = baseUrl, RelativeFileName = source.FileUrl, Type = "Brands" });
                        }
                    }
                }
            }

            TotalDownloadCount += links.Count;
            logger.Debug($"Brands Download Source Total is {links.Count}");
            return links;
        }
        public async Task AssembleBrandsData()
        {
            //IsLocalOnly = True, if Local Data is miss,need to load from web.
            logger.Debug("AssembleBrandsData");
            if (_settings.Products.IsLocalOnly || !_resourceUpdateProvider.BrandsHasUpdate)
            {
                logger.Debug(_brandJsonFullPath);
                if (ExistLocalData(_brandJsonFullPath))
                {
                    Brands = ReadBrandsDataFromLocal();
                }
                else
                {
                    Brands = await ReadBrandsDataFromWeb();
                }
            }
            //Read from web first, then from local.
            else
            {
                logger.Debug("ReadBrandsDataFromWeb");
                Brands = await ReadBrandsDataFromWeb();
                logger.Debug("ReadBrandsDataFromWeb Count" + Brands?.Count);
                if (IsBrandWebFailed)
                {
                    if (ExistLocalData(_brandJsonFullPath))
                    {
                        Brands = ReadBrandsDataFromLocal();
                    }
                }
            }
            IList<DownloadLink> newResources = ExtractBrandsLinks(SensingWebClient.MainServiceHost);
            _cacheManager.AddResources(newResources);
            if (!IsBrandWebFailed)
            {
                WriteDataToJsonFile(Brands, _brandJsonFullPath);
                _resourceUpdateProvider.BrandsUpdated();
            }

        }

        #endregion

        #region Properties

        private List<PropertyViewModel> _webPropertiesData = new List<PropertyViewModel>();
        private string _propertiesJsonFullPath;
        private string _webPropertiesJsonData;
        private ResourceUpdateTimeProvider _resourceUpdateProvider;

        public bool IsPropertiesWebFailed { get; set; } = true;

        public List<PropertyViewModel> PropertiesData
        {
            get { return _webPropertiesData; }
            set { _webPropertiesData = value; }
        }

        public async Task<List<PropertyViewModel>> ReadPropertiesDataFromWeb()
        {

            var data = await _webClient.GetProperties();
            if (data == null)
            {
                IsPropertiesWebFailed = true;
                return null;
            }
            IsPropertiesWebFailed = false;
            var tempData = new List<PropertyViewModel>(data.TotalCount);

            tempData.AddRange(data.Items);
            _webPropertiesJsonData = JsonConvert.SerializeObject(tempData);
            return tempData;
        }

        public List<PropertyViewModel> ReadPropertiesDataFromLocal()
        {
            var jsonFile = _propertiesJsonFullPath;
            if (File.Exists(jsonFile))
            {
                var jsondata = File.ReadAllText(jsonFile);
                return JsonConvert.DeserializeObject<List<PropertyViewModel>>(jsondata);
            }
            return null;
        }

        public virtual IList<DownloadLink> ExtractPropertiesLinks(string baseUrl)
        {
            return null;
        }
        public async Task AssemblePropertiesData()
        {
            //IsLocalOnly = True, if Local Data is miss,need to load from web.
            if (_settings.Products.IsLocalOnly || !_resourceUpdateProvider.PropertiesHasUpdate)
            {
                if (ExistLocalData(_propertiesJsonFullPath))
                {
                    PropertiesData = ReadPropertiesDataFromLocal();
                }
                else
                {
                    PropertiesData = await ReadPropertiesDataFromWeb();
                }
            }
            //Read from web first, then from local.
            else
            {
                PropertiesData = await ReadPropertiesDataFromWeb();
                if (IsPropertiesWebFailed)
                {
                    if (ExistLocalData(_propertiesJsonFullPath))
                    {
                        PropertiesData = ReadPropertiesDataFromLocal();
                    }
                }
            }

            IList<DownloadLink> newResources = ExtractPropertiesLinks(SensingWebClient.MainServiceHost);
            if (!IsPropertiesWebFailed)
            {
                WriteDataToJsonFile(PropertiesData, _propertiesJsonFullPath);
                _resourceUpdateProvider.PropertiesUpdated();
            }
            _cacheManager.AddResources(newResources);

        }

        #endregion

        #region ActivityGames

        private List<ActivityGameDto> _webActivityGamesData = new List<ActivityGameDto>();
        private string _activityGamesJsonFullPath;
        private string _activityGamesJsonData;
        public bool IsActivityGamesWebFailed { get; set; } = true;

        public List<ActivityGameDto> ActivityGames
        {
            get { return _webActivityGamesData; }
            set { _webActivityGamesData = value; }
        }

        public async Task<List<ActivityGameDto>> ReadActivityGamesDataFromWeb()
        {
            var tempData = new List<ActivityGameDto>();
            var data = await _webClient.GetActivityGames();
            if (data == null)
            {
                IsActivityGamesWebFailed = true;
                return null;
            }
            IsActivityGamesWebFailed = false;

            tempData.AddRange(data);
            _activityGamesJsonData = JsonConvert.SerializeObject(tempData);
            return tempData;
        }

        public List<ActivityGameDto> ReadActivityGamesDataFromLocal()
        {
            var jsonFile = _activityGamesJsonFullPath;
            if (File.Exists(jsonFile))
            {
                var jsondata = File.ReadAllText(jsonFile);
                return JsonConvert.DeserializeObject<List<ActivityGameDto>>(jsondata);
            }
            return null;
        }


        public async Task AssembleActivityGamesData()
        {
            //IsLocalOnly = True, if Local Data is miss,need to load from web.
            logger.Debug("AssembleActivityGamesData");
            if (_settings.Products.IsLocalOnly)
            {
                logger.Debug(_activityGamesJsonFullPath);
                if (ExistLocalData(_activityGamesJsonFullPath))
                {
                    ActivityGames = ReadActivityGamesDataFromLocal();
                }
                else
                {
                    ActivityGames = await ReadActivityGamesDataFromWeb();
                }
            }
            //Read from web first, then from local.
            else
            {
                logger.Debug("AssembleActivityGamesDataWeb");
                ActivityGames = await ReadActivityGamesDataFromWeb();
                logger.Debug("AssembleActivityGamesDataWeb Count" + ActivityGames?.Count);
                if (IsActivityGamesWebFailed)
                {
                    if (ExistLocalData(_activityGamesJsonFullPath))
                    {
                        ActivityGames = ReadActivityGamesDataFromLocal();
                    }
                }
            }

            if (!IsActivityGamesWebFailed)
            {
                WriteDataToJsonFile(ActivityGames, _activityGamesJsonFullPath);
            }

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
}
