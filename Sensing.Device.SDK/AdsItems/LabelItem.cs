using System;
using System.Collections.Generic;
using System.Text;

namespace Sensing.SDK.AdsItems
{
    public class LabelItem : BaseItem
    {
        public string color { get; set; }
        public double fontSize { get; set; }
        public int letterSpacing { get; set; }
        public bool fontWeight { get; set; }
        public bool fontStyle { get; set; }
        public int textIndent { get; set; }
        public int lineHeight { get; set; }
        public string textAlign { get; set; }
        public string whiteSpace { get; set; }
        public string textContent { get; set; }
        public string displayName { get; set; }
    }
}
