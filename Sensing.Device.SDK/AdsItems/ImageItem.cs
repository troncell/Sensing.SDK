using System;
using System.Collections.Generic;
using System.Text;

namespace Sensing.SDK.AdsItems
{
    public class ImageItem : BaseItem,ILink 
    {
        public int borderRadius { get; set; }

        public string backgroundImage { get; set; }
        public double backgroundOpacity { get; set; }
        public string backgroundSize { get; set; }
        public string backgroundRepeat { get; set; }


        IEnumerable<string> ILink.ExtractLinks()
        {
            yield return backgroundImage;
        }
    }
}
