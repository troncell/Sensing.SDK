using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sensing.SDK.Contract
{
    public class DateMetaphysicsDto 
    {
        public long Id { get; set; }

        public int? TenantId { get; set; }

        public long MetaphysicsId { get; set; }

        public DateTime Date { get; set; }

        public IList<LuckDto> Lucks { get; set; }

        public IList<RecommendDto> Recommneds { get; set; }

        /// <summary>
        /// 扩展字段，如黄历的数据.
        /// </summary>
        public string ExtensionData { get; set; }

        /// <summary>
        /// 占星师
        /// </summary>
        public string From { get; set; }

        public string FromLogoUrl { get; set; }
    }

    public class RecommendDto
    {
        public long Id { get; set; }
        public string Type { get; set; }
        public string ThingId { get; set; }
        public string Reason { get; set; }
        public int? TenantId { get; set; }
        public long DateMetaphysicsId { get; set; }
    }

    public class LuckDto
    {
        public long Id { get; set; }
        public int? TenantId { get; set; }

        public string Type { get; set; }
        public string Number { get; set; }
        public string NumberPresummary { get; set; }
        public string Color { get; set; }
        public string ColorPresummary { get; set; }
        public string Direction { get; set; }
        public string DirectionPresummary { get; set; }
        /// <summary>
        /// 整体运势
        /// </summary>
        public string Summary { get; set; }
        public string SummaryPresummary { get; set; }

        public string Love { get; set; }
        public string LovePresummary { get; set; }

        public string Career { get; set; }
        public string CareerPresummary { get; set; }

        public string Fortune { get; set; }
        public string FortunePresummary { get; set; }

        public string Health { get; set; }
        public string HealthPresummary { get; set; }

        /// <summary>
        /// 最佳匹配星座
        /// </summary>
        public string BestMatch { get; set; }

        public long DateMetaphysicsId { get; set; }
        public string Keyword { get; set; }
    }
}
