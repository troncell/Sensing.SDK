using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sensing.SDK.Contract
{
    public class TableLastTimeDto
    {
        public DateTime? Ads { get; set; }
        public DateTime? Apps { get; set; }
        public DateTime? ProductCategories { get; set; }
        public DateTime? Products { get; set; }
        public DateTime? Skus { get; set; }

        public DateTime? Tags { get; set; }
        public DateTime? ProductComments { get; set; }
        public DateTime? Properties { get; set; }
        public DateTime? PropertyValues { get; set; }
        public DateTime? Coupons { get; set; }
        public DateTime? Staffs { get; set; }
        public DateTime? MatchInfos { get; set; }
        public DateTime? LikeInfos { get; set; }
        public DateTime? Brands { get; set; }
    }

}
