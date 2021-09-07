using Sensing.SDK.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace SensingStoreCloud.Activity
{
    public enum AwardType
    {
        Coupon,
        Product,
        Placeholder
    }

    public class AwardOutput : EntityDto<long>
    {
        /// <summary>
        /// 所属的租户 Id
        /// </summary>
        public int TenantId { get; set; }

        /// <summary>
        /// 活动id
        /// </summary>
        public long ActivityId { get; set; }

        /// <summary>
        /// 计划奖品数
        /// </summary>
        public int PlanQty { get; set; }

        /// <summary>
        /// 已发奖品数
        /// </summary>
        public int ActualQty { get; set; }

        /// <summary>
        /// 奖项名称 如 1等奖，2等奖
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 奖品等级 如 1，2，对应上面的 1等奖，2等奖，以后排序用
        /// </summary>
        public int AwardSeq { get; set; }

        /// <summary>
        /// 奖品名称，如 ipad，索尼照相机
        /// 
        /// </summary>
        public string AwardProduct { get; set; }

        /// <summary>
        /// 奖品图片
        /// </summary>
        public string AwardImagePath { get; set; }

        /// <summary>
        /// 中奖概率
        /// </summary>
        public double Rate { get; set; }

        /// <summary>
        /// 最低中奖分数
        /// </summary>
        public int MinScore { get; set; }

        /// <summary>
        /// 最高中奖分数
        /// </summary>
        public int MaxScore { get; set; }

        /// <summary>
        /// Random Award Switch. Not used now.
        /// </summary>
        public bool IsRandomAward { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 奖品类型
        /// </summary>
        public AwardType Type { get; set; }
    }
}
