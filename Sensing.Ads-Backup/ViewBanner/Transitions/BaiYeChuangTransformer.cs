using System.Windows;
using System.Windows.Media;

namespace SensingAds.ViewBanner.Transitions
{
    public class BaiYeChuangTransformer : PageTransformer
    {
        public static readonly string Name = "BaiYeChuang";
        private int lines = 6;

        private PathGeometry path = new PathGeometry();

        public BaiYeChuangTransformer(int lines)
        {
            this.lines = lines;
        }

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
                //计算百叶窗每一行当前需要展示的左上右下
                for (int i = 0; i < lines; i++)
                {
                    double top = pageHeight / lines * i;
                    double bottom =  pageHeight / lines * position;
                    Rect r = new Rect(0,top,pageWidth, bottom);
                    RectangleGeometry box = new RectangleGeometry(r);
                    path.AddGeometry(box);
                }
                ((Banner)view).setClipPath(path);
            }
        }

        public string GetName()
        {
            return Name;
        }
    }
}
