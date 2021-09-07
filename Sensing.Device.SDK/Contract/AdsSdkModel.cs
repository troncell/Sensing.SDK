using System;
using System.Collections.Generic;
using System.Text;

namespace Sensing.SDK.Contract
{
    public class AdsSdkModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string Pixel { get; set; }
        public double Size { get; set; }
        public string FileUrl { get; set; }
        public string ThumbnailUrl { get; set; }
        public string Tags { get; set; }
        public string ProductAttributes { get; set; }
        public long[] TagIds { get; set; }
        public bool IsFromOthers { get; set; }

        public string AgeScope { get; set; }

        /// <summary>
        /// 显示Entity的排序顺序.
        /// </summary>
        public int OrderNumber { get; set; }

        /// <summary>
        /// Sku 针对性别
        /// 例：男=Male，女=Female 。为空代表无针对。
        /// </summary>
        public string Gender { get; set; }

        public string ExtraInfo { get; set; }

        /// <summary>
        /// 视频的情况下，指定开始播放的时间
        /// 例：比如从 1:08 处开始播放
        /// </summary>
        public DateTime? StartTime { get; set; }

        /// <summary>
        /// 广告的播放时长
        /// </summary>
        public TimeSpan? TimeSpan { get; set; }

        /// <summary>
        /// 页面切换的动画。
        /// </summary>
        public string Transition { get; set; }

        public string Description { get; set; }
        public bool IsCustom { get; set; }
        public string CustomContent { get; set; }
        public string AuditStatus { get; set; }
    }
}
