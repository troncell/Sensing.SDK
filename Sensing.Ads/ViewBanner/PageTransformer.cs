using System.Windows;

namespace SensingAds.ViewBanner
{
    public interface PageTransformer
    {
        string GetName();

        void OnAnimationStart(FrameworkElement view, double position);
        void OnAnimationEnd(FrameworkElement view, double position);
        void OnAnimationUpdate(FrameworkElement view, double position);

    }
}
