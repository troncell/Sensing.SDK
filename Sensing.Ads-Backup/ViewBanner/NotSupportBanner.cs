namespace SensingAds.ViewBanner
{
    public class NotSupportBanner : Banner
    {

        public NotSupportBanner(string filePath, int defaultDurtion)
        {
            setDefaultDurtion(defaultDurtion);
            
        }


        public override void Play()
        {
            base.Play();
            bannerState = BannerState.Prepared;
            ShowError($"文件不支持:{Title}");
        }
    }
}
