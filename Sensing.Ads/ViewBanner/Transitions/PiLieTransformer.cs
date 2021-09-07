using System.Windows;
using System.Windows.Media;

namespace SensingAds.ViewBanner.Transitions
{
    public class PiLieTransformer : PageTransformer
    {

        public static readonly string Name = "PiLie";


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
            double h = view.ActualHeight;
            double w = view.ActualWidth;
            if (position > 0)
            {
                view.Opacity = 1;
                path.Clear();
                double rectLeft = w / 2 * position;
                double rectRight = w - w / 2 * position;
                path.AddGeometry(new RectangleGeometry(new Rect(0,0,rectLeft,h)));
                path.AddGeometry(new RectangleGeometry(new Rect(rectRight, 0, w - rectLeft, h)));
                ((Banner)view).setClipPath(path);
            }
        }

        public string GetName()
        {
            return Name;
        }
    }
}
