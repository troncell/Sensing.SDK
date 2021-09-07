using System;
using System.Collections.Generic;
using System.Text;

namespace Sensing.SDK.AdsItems
{
    public class VideoItem : BaseItem,ILink 
    {
        public bool autoplay { get; set; }
        public bool loop { get; set; }
        public bool controls { get; set; }
        public bool muted { get; set; }
        public string resource { get; set; }

        IEnumerable<string> ILink.ExtractLinks()
        {
            yield return resource;
        }
    }
}
