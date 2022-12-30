using AppPod.Setting;
using LogService;
using Newtonsoft.Json;
using Sensing.SDK;
using Sensing.SDK.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AppPod.DataAccess.WebData.Store
{
    public class AdsSchedulingApiCacheDataProvider : SingleCachedListDataProviderBase<AdAndAppTimelineScheduleViewModel>
    {
        protected readonly IBizLogger logger = ServerLogFactory.GetLogger(typeof(ProductsApiCacheDataProvider));

        private SensingWebClient _webClient;
        private Settings _settings;


        public AdsSchedulingApiCacheDataProvider(SensingWebClient webClient, string appPodDataFolder, Settings settings, DownloadAlgorithm algorithm = DownloadAlgorithm.WebClient)
            : base(appPodDataFolder, "AdAndApp_Timelines.json", algorithm)
        {
            _webClient = webClient;
            _settings = settings;
        }

        public override IList<DownloadLink> ExtractLinks(string baseUrl)
        {
            if (Data == null) return null;
            List<DownloadLink> links = new List<DownloadLink>();
            return links;
        }

        public override bool HasUpdate()
        {
            return true;
        }

        public override bool IsLocalOnly()
        {
            return false;
        }

        public override async Task<List<AdAndAppTimelineScheduleViewModel>> ReadDataFromWeb()
        {
            var data = await _webClient.GetAdAndAppTimelinesInAWeek(DateTime.Now);
            if (data == null)
            {
                IsWebFailed = true;
                return null;
            }
            IsWebFailed = false;
            return data;
        }

        public override void TimeUpdated()
        {
            _resourceUpdateProvider.AdsUpdated();
        }
    }
}
