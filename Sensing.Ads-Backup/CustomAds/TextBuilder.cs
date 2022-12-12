using Newtonsoft.Json;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace SensingAds.CustomAds
{
    public class TextBuilder : ViewBuilder
    {
        public String textContent;
        public String textAlign = "center";
        public int lineHeight;
        public int fontSize;
        public bool fontWeight;
        public String color = "#ffffff";
        public override FrameworkElement Build()
        {
            var textView = new TextBlock();
            setCommonProperties(textView);
            //dpi
            textView.FontSize = fontSize;
            if (textAlign.Equals("center"))
            {
                textView.TextAlignment = TextAlignment.Center;
            }
            try
            {
                BrushConverter b = new BrushConverter();
                textView.Foreground = (SolidColorBrush)b.ConvertFromString(color);
            }
            catch (Exception ex) { }
            textView.Text = textContent;
            return textView;
        }

    }
}
