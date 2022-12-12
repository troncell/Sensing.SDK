using System.Collections.Generic;

namespace SensingAds.ViewBanner.AdsItem
{
    public interface ILink
    {
        IEnumerable<string> ExtractLinks();
    }
}
