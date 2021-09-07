using System.Windows;
using System.Windows.Media;

namespace SensingAds.ViewBanner.Transitions
{
    public class HeZhuangTransformer : PageTransformer
    {
        public static readonly string Name = "HeZhuang";

        private int lines = 6;

        private PathGeometry path = new PathGeometry();

        public void OnAnimationStart(FrameworkElement view, double position)
        {
        }

        public void OnAnimationEnd(FrameworkElement view, double position)
        {
            ((Banner)view).setClipPath(null);
        }

        public void OnAnimationUpdate(FrameworkElement view, double position)
        {
            double pageHeight = view.ActualHeight;
            double pageWidth = view.ActualWidth;
            if (position > 0)
            {
                view.Opacity = 1;
                path.Clear();
                double left = (pageWidth / 2 * (1 - position));
                double right = pageWidth - left;
                double top = (pageHeight / 2 * (1 - position));
                double bottom = pageHeight - top;
                RectangleGeometry box = new RectangleGeometry(new Rect(left,top,right - left,bottom - top));
                path.AddGeometry(box);
                ((Banner)view).setClipPath(path);
            }
        }

        public string GetName()
        {
            return Name;
        }
    }
}
