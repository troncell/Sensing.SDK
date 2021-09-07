using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Sensing.SDK.AdsItems
{
    public class SwiperItem : BaseItem, ILink
    {
        public int resourceListIndex { get; set; }
        public List<ResourceListItem> resourceList { get; set; }

        IEnumerable<string> ILink.ExtractLinks()
        {
            foreach (var item in resourceList)
            {
                yield return item.resource;
            }
        }
    }

    public class ResourceListItem
    {
        public bool showDetail { get; set; }
        public string type { get; set; }
        public string resource { get; set; }
    }
}
