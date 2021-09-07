using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sensing.SDK.AdsItems
{
    public abstract class BaseItem 
    {
        public string name { get; set; }
        public string type { get; set; }
        public double width { get; set; }
        public double height { get; set; }
        public double left { get; set; }
        public double top { get; set; }
        public bool isShow { get; set; }
        public double rotate { get; set; }
        public int zIndex { get; set; }
        public string backgroundColor { get; set; }
        public string transformScale { get; set; }

    }
}
