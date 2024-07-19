using AppPod.Data.DataProviders;
using AppPod.DataAccess.Helper;
using AppPod.Setting;
using LogService;
using Newtonsoft.Json;
using Sensing.SDK;
using Sensing.SDK.AdsItems;
using Sensing.SDK.Contract;
using SensingBase.Utils;
using SensingHub.Sdk;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace AppPod.DataAccess.WebData.Store
{
    public class HubResourceApiCacheDataProvider : SingleCachedDataProviderBase<DeviceInfo>
    {
        protected readonly IBizLogger logger = ServerLogFactory.GetLogger(typeof(HubResourceApiCacheDataProvider));

        private SensingHubClient _webClient;
        private Settings _settings;


        public HubResourceApiCacheDataProvider(SensingHubClient webClient, string hubBaseFolder, Settings settings, DownloadAlgorithm algorithm = DownloadAlgorithm.WebClient)
            : base(hubBaseFolder, "HubData.json", algorithm)
        {
            _webClient = webClient;
            _settings = settings;
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
            //await AssembleTagsData();
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

        public bool ExistLocalData(string jsonPath)
        {
            return File.Exists(jsonPath);
        }

        public override IList<DownloadLink> ExtractLinks(string baseUrl)
        {
            if (Data == null) return null;
            List<DownloadLink> links = new List<DownloadLink>();
            if (Data.Resources != null)
            {
                foreach (var res in Data.Resources)
                {
                    if (res.FileType == "H5") continue;
                    links.Add(new DownloadLink { Host = baseUrl, RelativeFileName = res.FileUrl, Type = "hub", Md5 = res.MD5 });
                }
            }
            TotalDownloadCount = links.Count;
            logger.Debug($"Sensing Hub Download Source Total is {TotalDownloadCount}");
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

        public override async Task<DeviceInfo> ReadDataFromWeb()
        {

            var pageSize = 300;
            var skipCount = 0;
            var data = await _webClient.GetDeviceResourcesAsync();
            if (data == null)
            {
                IsWebFailed = true;
                return null;
            }
            IsWebFailed = false;
            //var maxCount = data.TotalCount;
            //var tempData = new List<DeviceInfo>();
            //tempData.AddRange(data.Items);

            //while (tempData.Count < maxCount)
            //{
            //    skipCount = tempData.Count;
            //    var newData = await _webClient.GetAds(skipCount, pageSize);
            //    if (newData == null) break;
            //tempData.Add(data);
            //}

            //_webData = tempData;
            return data;
        }

        public override void TimeUpdated()
        {
            _resourceUpdateProvider.AdsUpdated();
        }
    }
}
