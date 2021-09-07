namespace SensingAds.ViewBanner.AdsItem
{
    public class ImageAd : BaseAd
    {
        public override Banner CreateBanner()
        {
            ImageBanner banner = new ImageBanner(FileUrl, TimeSpan);
            banner.Title = this.Name;
            banner.Id = Id;
            banner.Transition = Transition;
            return banner;
        }
    }
}
