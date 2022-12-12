using System.Windows;
using System.Windows.Media;

namespace SensingAds.ViewBanner.Transitions
{
    public class FadeInTransformer : PageTransformer
    {
        public static readonly string Name = "FadeIn";


        public void OnAnimationStart(FrameworkElement view, double position)
        {
        }

        public void OnAnimationEnd(FrameworkElement view, double position)
        {
        }

        public void OnAnimationUpdate(FrameworkElement view, double position)
        {
            if (position > 0)
            {
                view.Opacity = position;
            }
        }

        public string GetName()
        {
            return Name;
        }
    }
}
