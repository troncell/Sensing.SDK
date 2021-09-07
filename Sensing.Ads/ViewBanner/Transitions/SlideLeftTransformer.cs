using System.Windows;
using System.Windows.Media;

namespace SensingAds.ViewBanner.Transitions
{
    public class SlideLeftTransformer : PageTransformer
    {
        public static readonly string Name = "SlideLeft";


        public void OnAnimationStart(FrameworkElement view, double position)
        {
            view.RenderTransform = new TranslateTransform();
        }

        public void OnAnimationEnd(FrameworkElement view, double position)
        {
            ((TranslateTransform)view.RenderTransform).X = 0;

        }



        public void OnAnimationUpdate(FrameworkElement view, double position)
        {
            double pageWidth = view.ActualWidth;
            if (position > 0)
            {
                //view.setAlpha(1);
                view.Opacity = 1;
                //view.setTranslationY(pageHeight * (1 - position));
                ((TranslateTransform)view.RenderTransform).X = pageWidth * (1 - position);
            }
            else
            {
                //view.setTranslationY(pageHeight * position);
                ((TranslateTransform)view.RenderTransform).X = pageWidth * position;

            }
        }

        public string GetName()
        {
            return Name;
        }
    }
}
