using System.Collections.Generic;

namespace Sensing.SDK.AdsItems
{
    public interface ILink
    {
        IEnumerable<string> ExtractLinks();
    }
}
