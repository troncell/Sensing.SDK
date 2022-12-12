using System.Collections.Generic;
using System.Linq;

namespace SensingAds.ViewBanner.AdsItem
{
    public class OfflineAd : BaseAd
    {
        public override Banner CreateBanner()
        {
            OfflineBanner banner = new OfflineBanner(FileUrl, TimeSpan);
            banner.Title = this.Name;
            banner.Id = Id;
            return banner;
        }

    }
}
