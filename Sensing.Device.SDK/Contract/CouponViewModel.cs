using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sensing.SDK.Contract
{
    public class CouponViewModel
    {
        public long Id { get; set; }
        public int TenantId { get; set; }

        public long? OrganizationUnitId { get; set; }

        public int OrderNumber { get; set; }

        /// <summary>
        /// 优惠券其他平台id
        /// </summary>
        public int OuterId { get; set; }

        /// <summary>
        /// 面额
        /// </summary>

        /// <summary>
        /// 优惠券活动ID 应该是模板id
        /// </summary>
        public string SpreadId { get; set; }

        /// <summary>
        /// 优惠券名称
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 优惠券图片
        /// </summary>
        public string Pictures { get; set; }


        /// <summary>
        /// 优惠券生效时间
        /// </summary>
        public DateTime Start_time { get; set; }

        /// <summary>
        /// 优惠券的截止日期
        /// </summary>
        public DateTime End_time { get; set; }

        /// <summary>
        /// 优惠券地址
        /// </summary>
        public String Url { get; set; }

        /// <summary>
        /// 外部定义的
        /// </summary>
        public String Code { get; set; }
    }
}
