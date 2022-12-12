namespace SensingAds.ViewBanner
{
    public class OfflineBanner : Banner
    {

        public OfflineBanner(string filePath, int defaultDurtion)
        {
            setDefaultDurtion(defaultDurtion);
            
        }


        public override void Play()
        {
            base.Play();
            bannerState = BannerState.Prepared;
            ShowError($"该广告已下线:{Title}");
        }
    }
}
