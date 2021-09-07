using System;
using System.Collections.Generic;

namespace SensingAds.ViewBanner.AdsItem
{
    public abstract class BaseAd : ILink
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string FileUrl { get; set; }
        public int TimeSpan { get; set; }
        public bool IdleAble { get; set; }
        public string Transition { get; set; }
        public virtual IEnumerable<string> ExtractLinks()
        {
            yield return FileUrl;
        }

        public virtual Banner CreateBanner()
        {
            NotSupportBanner banner = new NotSupportBanner(FileUrl, TimeSpan);
            banner.Title = this.Name;
            banner.Id = Id;
            return banner;
        }
    }
}
