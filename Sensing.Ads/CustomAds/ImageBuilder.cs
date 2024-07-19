using SensingAds.Uitl;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace SensingAds.CustomAds
{
    public class ImageBuilder : ViewBuilder
    {
        public String backgroundImage;
        //100% 100% 撑满
        //cover 覆盖
        //contain 填充
        //
        public string backgroundSize;

        public override FrameworkElement Build()
        {
            Image imageView = new Image();
            setCommonProperties(imageView);
            if(backgroundSize == "100% 100%")
            {
                imageView.Stretch = System.Windows.Media.Stretch.Fill;
            }
            else if(backgroundSize == "cover")
            {
                imageView.Stretch = System.Windows.Media.Stretch.UniformToFill;
            }
            if (!string.IsNullOrEmpty(backgroundImage))
            {
                ImageLoaderUtil.Load(backgroundImage, imageView);
            }
            return imageView;
        }

        public override IEnumerable<string> ExtractLinks()
        {
            yield return backgroundImage;
        }
    }
}
