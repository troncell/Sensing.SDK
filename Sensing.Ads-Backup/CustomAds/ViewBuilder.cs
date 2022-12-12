using SensingAds.ViewBanner.AdsItem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace SensingAds.CustomAds
{
    public abstract class ViewBuilder : ILink
    {
        public int zIndex;
        public int width;
        public int height;
        public int left;
        public int top;
        public int rotate;
        public String displayName;
        public String extensionData;

        public void setCommonProperties(FrameworkElement view)
        {
            Canvas.SetLeft(view, left);
            Canvas.SetTop(view, top);
            Panel.SetZIndex(view, zIndex);
            view.Width = width;
            view.Height = height;
            if (!string.IsNullOrEmpty(displayName))
            {
                //int resourceId = context.getResources().getIdentifier(displayName, "id", context.getPackageName());
                //if (resourceId != 0)
                //    view.setId(resourceId);
            }
        }


        public abstract FrameworkElement Build();

        public virtual void Start() { }

        public virtual void Stop() { }

        public virtual IEnumerable<string> ExtractLinks()
        {
            return Enumerable.Empty<string>();
        }
    }
}
