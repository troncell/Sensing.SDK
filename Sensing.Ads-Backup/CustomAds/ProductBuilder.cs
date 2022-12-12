using Sensing.Ads;
using SensingAds.Uitl;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace SensingAds.CustomAds
{
    public class ProductBuilder : ViewBuilder
    {
        public int productId { get; set; }

        public override FrameworkElement Build()
        {
            Button button = new Button();
            setCommonProperties(button);
            button.Click += Control_Click;
            button.Opacity = 0;
            return button;
        }

        private void Control_Click(object sender, RoutedEventArgs e)
        {
            AdsEventHelper.AdsProductEvent?.Invoke(sender, productId);
        }
    }
}
