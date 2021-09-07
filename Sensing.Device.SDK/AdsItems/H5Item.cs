using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Sensing.SDK.AdsItems
{
    public class H5Item : BaseItem, ILink
    {
        public string resource { get; set; }
        public string downloadMode { get; set; }

        IEnumerable<string> ILink.ExtractLinks()
        {
            if(downloadMode == "self")
            {
                yield return resource;
            }
            yield return null;
        }
    }

}
