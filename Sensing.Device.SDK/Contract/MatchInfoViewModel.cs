using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sensing.SDK.Contract
{
    public class MatchInfoViewModel
    {
        public long Id { get; set; }

        public int TenantId { get; set; }

        public long? OrganizationUnitId { get; set; }

        public string Code { get; set; }

        public int OrderNumber { get; set; }

        /// <summary>
        /// 展示图
        /// </summary>
        public string ShowImage { get; set; }

        /// <summary>
        /// 搭配效果大图
        /// </summary>
        public string BImg { get; set; }
        /// <summary>
        /// 搭配效果中图
        /// </summary>

        public string MImg { get; set; }
        /// <summary>
        /// 搭配效果小图
        /// </summary>


        public string SImg { get; set; }

        // public  ICollection<MatchItem> MatchItems { get; set; }


        public string Name { get; set; }

        public ICollection<MatchItemViewModel> MatchItems { get; set; }

        public string Description { get; set; }
    }

    public class MatchItemViewModel
    {
        public long Id { get; set; }
        public long MatchInfoId { get; set; }
        public long SkuId { get; set; }
        public string SkuPicUrl { get; set; }
        public string SkuTitle { get; set; }

        public string Reason { get; set; }
        public bool IsMain { get; set; }
    }
}
