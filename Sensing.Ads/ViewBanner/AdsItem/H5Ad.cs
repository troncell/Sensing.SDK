using SensingAds.ViewBanner.Transitions;
using System.Collections.Generic;
using System.Linq;

namespace SensingAds.ViewBanner.AdsItem
{
    public class H5Ad : BaseAd
    {
        public DownloadMode DownloadMode { get; set; }
        public override IEnumerable<string> ExtractLinks()
        {
            if (DownloadMode == DownloadMode.Self)
                return Enumerable.Repeat<string>(FileUrl, 1);
            return Enumerable.Empty<string>();
        }

        public override Banner CreateBanner()
        {
            H5Banner banner = new H5Banner(FileUrl, TimeSpan);
            banner.Title = this.Name;
            banner.Id = Id;
            banner.Transition = NoneTransformer.Name;
            return banner;
        }

       
    }

    public enum DownloadMode
    {
        None,
        Self,
        All
    }
}
