using System;
using System.Collections.Generic;
using System.Text;

namespace Sensing.SDK.Contract
{
    public class TagSdkModel
    {
        public long Id { get; set; }

        public string Value { get; set; }

        public string Type { get; set; }

        public bool IsSpecial { get; set; }

        public string IconUrl { get; set; }
    }
}
