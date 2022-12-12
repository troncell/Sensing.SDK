﻿using System.Windows;
using System.Windows.Media;

namespace SensingAds.ViewBanner.Transitions
{
    public class SlideDownTransformer : PageTransformer
    {
        public static readonly string Name = "SlideDown";


        public void OnAnimationStart(FrameworkElement view, double position)
        {
            view.RenderTransform = new TranslateTransform();
        }

        public void OnAnimationEnd(FrameworkElement view, double position)
        {
            ((TranslateTransform)view.RenderTransform).Y = 0;

        }



        public void OnAnimationUpdate(FrameworkElement view, double position)
        {
            double pageHeight = view.ActualHeight;
            if (position > 0)
            {
                //view.setAlpha(1);
                view.Opacity = 1;
                //view.setTranslationY(pageHeight * (1 - position));
                ((TranslateTransform)view.RenderTransform).Y = pageHeight * (position - 1);
            }
            else
            {
                //view.setTranslationY(pageHeight * position);
                ((TranslateTransform)view.RenderTransform).Y = pageHeight * -position;
            }
        }

        public string GetName()
        {
            return Name;
        }
    }
}
