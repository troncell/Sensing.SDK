using SensingAds.ViewBanner.Transitions;

namespace SensingAds.ViewBanner.AdsItem
{
    public class WindowAppAd : BaseAd
    {
        public override Banner CreateBanner()
        {
            ExeBanner banner = new ExeBanner(FileUrl, TimeSpan);
            banner.Title = this.Name;
            banner.Id = Id;
            banner.Transition = NoneTransformer.Name;
            return banner;
        }
    }
}
