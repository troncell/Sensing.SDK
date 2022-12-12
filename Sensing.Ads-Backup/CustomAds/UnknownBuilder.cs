using System.Windows;
using System.Windows.Controls;

namespace SensingAds.CustomAds
{
    public class UnknownBuilder : TextBuilder
    {
        public override FrameworkElement Build()
        {
            TextBlock textView = new TextBlock();
            setCommonProperties(textView);
            textView.Text = "未知元素";
            return textView;
        }
    }
}
