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
    public class AppPodVersionApiCacheDataProvider : SingleCachedListDataProviderBase<DeviceAppPodVersionModel>
    {
        protected readonly IBizLogger logger = ServerLogFactory.GetLogger(typeof(AppPodVersionApiCacheDataProvider));

        private SensingWebClient _webClient;
        private Settings _settings;
        public AppPodVersionApiCacheDataProvider(SensingWebClient webClient, string metaBaseFolder, Settings settings, DownloadAlgorithm algorithm = DownloadAlgorithm.WebClient)
            : base(metaBaseFolder, "AppPodVersion.json", algorithm)
        {
            _webClient = webClient;
            _settings = settings;
        }

        public override IList<DownloadLink> ExtractLinks(string baseUrl)
        {
            if (Data == null) return null;

            baseUrl = SensingWebClient.RecommendApiHost;
            List<DownloadLink> links = new List<DownloadLink>();

            return links;
        }

        public override async Task<List<DeviceAppPodVersionModel>> ReadDataFromWeb()
        {
            var data = await _webClient.GetDeviceAppPodVersion();
            if (data == null)
            {
                IsWebFailed = true;
                return null;
            }
            IsWebFailed = false;
            return new List<DeviceAppPodVersionModel> { data };
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

}