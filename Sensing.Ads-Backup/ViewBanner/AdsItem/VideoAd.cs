namespace SensingAds.ViewBanner.AdsItem
{
    public class VideoAd : BaseAd
    {
        public override Banner CreateBanner()
        {
            VideoBanner banner = new VideoBanner(FileUrl, TimeSpan);
            banner.Title = this.Name;
            banner.Id = Id;
            banner.Transition = Transition;
            return banner;
        }
    }
}
