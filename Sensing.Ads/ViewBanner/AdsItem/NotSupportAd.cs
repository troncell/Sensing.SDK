using System.Collections.Generic;
using System.Linq;

namespace SensingAds.ViewBanner.AdsItem
{
    public class NotSupportAd : BaseAd
    {
        public override IEnumerable<string> ExtractLinks()
        {
            return Enumerable.Empty<string>();
        }


    }
}
