namespace SensingAds.ViewBanner
{
    public class EmptyBanner : Banner
    {

        public EmptyBanner(string filePath, int defaultDurtion)
        {
            setDefaultDurtion(defaultDurtion);
            
        }


        public override void Play()
        {
            base.Play();
            bannerState = BannerState.Prepared;
            ShowError($"空闲");
        }
    }
}
