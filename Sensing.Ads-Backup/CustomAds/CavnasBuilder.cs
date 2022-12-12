using SensingAds.Uitl;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace SensingAds.CustomAds
{
    public class CavnasBuilder : ViewBuilder
    {
        public String backgroundImage;
        public string backgroundColor = "#ffffff";

        public override FrameworkElement Build()
        {
            Canvas canvas = new Canvas();
            setCommonProperties(canvas);
            if (!string.IsNullOrEmpty(backgroundImage))
            {
                ImageLoaderUtil.LoadBackground(backgroundImage, canvas);
            }
            else
            {
                try
                {
                    BrushConverter b = new BrushConverter();
                    canvas.Background = (SolidColorBrush)b.ConvertFromString(backgroundColor);
                }
                catch (Exception ex) { }
            }
            return canvas;
        }

        public override IEnumerable<string> ExtractLinks()
        {
            yield return backgroundImage;

        }
    }
}
