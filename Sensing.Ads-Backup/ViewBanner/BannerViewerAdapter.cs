using SensingAds.ViewBanner.AdsItem;
using System.Collections.Generic;

namespace SensingAds.ViewBanner
{
    public class BannerViewerAdapter
    {
        private int defaultPlayTime;
        private List<BaseAd> ads = new List<BaseAd>();

        public BannerViewerAdapter(int defaultPlayTime)
        {
            this.defaultPlayTime = defaultPlayTime;
        }

        public void UpdateData(List<BaseAd> ads)
        {
            this.ads = ads;
        }

        public Banner Get(int index)
        {
            return ads[index].CreateBanner();
        }

        public int Size()
        {
            return ads.Count;
        }
    }
}
