using AppPod.Data.DataProviders;
using AppPod.DataAccess.Helper;
using AppPod.Setting;
using LogService;
using Newtonsoft.Json;
using Sensing.SDK;
using Sensing.SDK.AdsItems;
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
    public class AdsApiCacheDataProvider : SingleCachedListDataProviderBase<AdsSdkModel>
    {
        protected readonly IBizLogger logger = ServerLogFactory.GetLogger(typeof(ProductsApiCacheDataProvider));

        private SensingWebClient _webClient;
        private Settings _settings;


        public AdsApiCacheDataProvider(SensingWebClient webClient, string adsBaseFolder, Settings settings, DownloadAlgorithm algorithm = DownloadAlgorithm.WebClient)
            : base(adsBaseFolder, "Ads.json", algorithm)
        {
            _webClient = webClient;
            _settings = settings;
            _tagsJsonFullPath = Path.Combine(adsBaseFolder, "AdTags.json");
        }


        public override async Task Initialize(ResourceUpdateTimeProvider resourceUpdateProvider)
        {
            try
            {
                _resourceUpdateProvider = resourceUpdateProvider;
                if (_algorithm == DownloadAlgorithm.WebClient)
                {
                    _cacheManager = new WebClientLoalResourceCacheManager(_baseFolder, "res", true);
                }
                if (_algorithm == DownloadAlgorithm.Mydownload)
                {
                    _cacheManager = new LocalResourcesCacheManager(_baseFolder, "res", true);
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


        /// <summary>
        /// Default is Cached
        /// </summary>
        protected override async Task InitData()
        {
            await AssembleData();
            await AssembleTagsData();
        }

        #region Tags

        private List<TagSdkModel> _webTagData = new List<TagSdkModel>();
        private string _tagsJsonFullPath;
        private string _webTagsJsonData;
        public bool IsTagsWebFailed { get; set; } = true;

        public bool IsDeviceTagsWebFailed { get; set; } = true;

        public List<TagSdkModel> TagsData
        {
            get { return _webTagData; }
            set { _webTagData = value; }
        }

        public async Task<List<TagSdkModel>> ReadTagsDataFromWeb()
        {
            var tempData = new List<TagSdkModel>();
            var data = await _webClient.GetAdsTags();
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

        public override IList<DownloadLink> ExtractLinks(string baseUrl)
        {
            if (Data == null) return null;
            List<DownloadLink> links = new List<DownloadLink>();
            foreach (var item in Data)
            {
                if (item.IsCustom && !string.IsNullOrEmpty(item.CustomContent))
                {
                    try
                    {
                        var customAd = CustomAdHelper.Parse(item.CustomContent, item.Id, item.Name, 0);
                        var contentLinks = customAd.ExtractLinks().Where(l => !string.IsNullOrEmpty(l));
                        foreach (var l in contentLinks)
                        {
                            if (!links.Any(link => link.RelativeFileName == l))
                            {
                                links.Add(new DownloadLink { Host = baseUrl, RelativeFileName = l, Type = "ads" });
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                    }
                }
                if (item.FileUrl != null)
                {
                    if (!links.Any(link => link.RelativeFileName == item.FileUrl))
                    {
                        links.Add(new DownloadLink { Host = baseUrl, RelativeFileName = item.FileUrl, Type = "ads" });
                    }
                }
                if (item.ThumbnailUrl != null)
                {
                    if (!links.Any(link => link.RelativeFileName == item.ThumbnailUrl))
                    {
                        links.Add(new DownloadLink { Host = baseUrl, RelativeFileName = item.ThumbnailUrl, Type = "ads" });
                    }
                }

            }
            TotalDownloadCount = links.Count;
            logger.Debug($"Ads Download Source Total is {TotalDownloadCount}");
            return links;
        }

        public override bool HasUpdate()
        {
            return _resourceUpdateProvider.AdsHasUpdate;
        }

        public override bool IsLocalOnly()
        {
            var result = _settings?.Ads.IsLocalOnly;
            return result.HasValue ? result.Value : false;
        }

        public override async Task<List<AdsSdkModel>> ReadDataFromWeb()
        {

            var pageSize = 300;
            var skipCount = 0;
            var data = await _webClient.GetAds(skipCount, pageSize);
            if (data == null)
            {
                IsWebFailed = true;
                return null;
            }
            IsWebFailed = false;
            var maxCount = data.TotalCount;
            var tempData = new List<AdsSdkModel>(maxCount);
            tempData.AddRange(data.Items);

            while (tempData.Count < maxCount)
            {
                skipCount = tempData.Count;
                var newData = await _webClient.GetAds(skipCount, pageSize);
                if (newData == null) break;
                tempData.AddRange(newData.Items);
            }

            //_webProductsJsonData = JsonConvert.SerializeObject(tempData);
            return tempData;
        }

        public override void TimeUpdated()
        {
            _resourceUpdateProvider.AdsUpdated();
        }
    }
}
