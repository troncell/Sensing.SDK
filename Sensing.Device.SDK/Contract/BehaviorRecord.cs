using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sensing.SDK.Contract
{
    public class BehaviorRecord
    {
        /// <summary>
        /// item的id
        /// </summary>
        public string ThingId { get; set; }
        public long ProductId { get; set; }
        /// <summary>
        /// item的名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 当前行为的类别，如Ads,Products,Apps等.
        /// </summary>
        public string Category { get; set; }

        /// <summary>
        /// 一段时间内的增量
        /// </summary>
        public int Increment { get; set; }

        /// <summary>
        /// 关注特定SKU的行为,如Click,Like,Take,See,Buy,ScanQrCode...
        /// </summary>
        public string Action { get; set; }


        /// <summary>
        /// 这个时间是设备传过来，最终的效果是 2016/09/19 10：00：05 这个时间点的增量，比如5分钟单位集合
        /// </summary>
        public DateTime CollectionTime { get; set; }

        /// <summary>
        /// 数据收集的结束时间.
        /// </summary>
        public DateTime CollectEndTime { get; set; }

        /// <summary>
        /// 来自于那个软件
        /// </summary>
        public string SoftwareName { get; set; }

        /// <summary>
        /// 额外的附件信息.
        /// </summary>
        public string Comments { get; set; }

        public string PageName { get; set; }
        public string PreviousPageName { get; set; }
        public string PreviousPageArea { get; set; }
    }
}
