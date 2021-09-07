
using SensingAds.Uitl;
using System.Windows.Controls;

namespace SensingAds.ViewBanner
{
    public class ImageBanner : Banner
    {
        private string localPath;

        public ImageBanner(string filePath, int defaultDurtion)
        {
            setDefaultDurtion(defaultDurtion);
            init(filePath);
        }
        private void init(string filePath)
        {
            localPath = filePath;
            Image image = new Image();
            image.Stretch = System.Windows.Media.Stretch.Uniform;
            ImageLoaderUtil.Load(filePath, image);
            this.Children.Add(image);
            bannerState = BannerState.Loaded;
        }

        public override void Play()
        {
            base.Play();
            bannerState = BannerState.Prepared;
        }
    }
}
