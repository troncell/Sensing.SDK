using Newtonsoft.Json;
using Sensing.SDK.Contract;
using SensingAds.ViewBanner;
using SensingAds.ViewBanner.AdsItem;
using System;
using System.Collections.Generic;
using System.Windows;

namespace SensingAds.CustomAds
{
    public class SwiperBuilder : ViewBuilder
    {
        [JsonIgnore]
        private BannerViewer bannerViewer;

        public int resourceListIndex { get; set; }
        public List<SwipeItem> resourceList { get; set; }
        public override FrameworkElement Build()
        {
            bannerViewer = new BannerViewer();
            var bannerAdpater = new BannerViewerAdapter(7);
            if(resourceList != null)
            {
                List<BaseAd> ads = new List<BaseAd>();
                foreach (var item in resourceList)
                {
                    var ad = AdsFactory.CreateAd(new AdsSdkModel
                    {
                        Type = item.type,
                        FileUrl = item.resource,
                        TimeSpan = TimeSpan.FromSeconds(5)
                    });
                    ads.Add(ad);
                }
                bannerAdpater.UpdateData(ads);
            }
            bannerViewer.SetAdapter(bannerAdpater);
            setCommonProperties(bannerViewer);
            return bannerViewer;
        }

        public override void Start()
        {
            base.Start();
            bannerViewer.Play();
        }

        public override void Stop()
        {
            base.Stop();
            bannerViewer.Stop();
        }

        public override IEnumerable<string> ExtractLinks()
        {
            foreach (var item in resourceList)
            {
                yield return item.resource;
            }
        }

        public class SwipeItem
        {
            public bool showDetail { get; set; }
            public string type { get; set; }
            public string resource { get; set; }
        }
    }
}
