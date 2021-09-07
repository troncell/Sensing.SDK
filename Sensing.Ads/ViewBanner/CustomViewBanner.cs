using SensingAds.CustomAds;
using System.Collections.Generic;
using System.Windows;

namespace SensingAds.ViewBanner
{
    public class CustomViewBanner : Banner
    {
        private List<ViewBuilder> viewBuilders;

        public CustomViewBanner(FrameworkElement view, List<ViewBuilder> viewBuilders, int defaultDurtion)
        {
            this.viewBuilders = viewBuilders;
            setDefaultDurtion(defaultDurtion);
            this.Children.Add(view);
        }

        public override void Play()
        {
            base.Play();
            bannerState = BannerState.Prepared;
            if (viewBuilders != null)
            {
                for (int i = 0; i < viewBuilders.Count; i++)
                {
                    ViewBuilder vb = viewBuilders[i];
                    vb.Start();
                }
            }
        }

        public override void Stop()
        {
            base.Stop();
            if (viewBuilders != null)
            {
                for (int i = 0; i < viewBuilders.Count; i++)
                {
                    ViewBuilder vb = viewBuilders[i];
                    vb.Stop();
                }
            }
        }

    }
}
