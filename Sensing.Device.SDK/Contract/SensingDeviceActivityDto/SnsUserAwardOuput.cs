using System;
using System.Collections.Generic;
using System.Text;

namespace SensingStoreCloud.Activity
{
    public class SnsUserAwardOuput
    {
        /// <summary>
        /// 所属的租户 Id
        /// </summary>
        public int TenantId { get; set; }

        public long ActivityId { get; set; }

        public long? UserActionId { get; set; }

        public long SnsUserInfoId { get; set; }

        public SnsUserInfoOutput SnsUserInfo { get; set; }

        public long AwardId { get; set; }

        public virtual AwardOutput Award { get; set; }

        /// <summary>
        /// 中奖通知消息是否成功发送
        /// </summary>
        public bool IsNotified { get; set; }

        /// <summary>
        /// 用户是否收到奖品
        /// </summary>
        public bool IsReceived { get; set; }

        /// <summary>
        /// 收货人电话
        /// </summary>
        public string Phone { get; set; }

        /// <summary>
        /// 收货人地址
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// 收货人姓名
        /// </summary>
        public string ReceiverName { get; set; }

        /// <summary>
        /// 快递单号
        /// </summary>
        public string ExpressNO { get; set; }

        /// <summary>
        /// 快递公司名称
        /// </summary>
        public string ExpressCompany { get; set; }

        /// <summary>
        /// 快递单照片
        /// </summary>
        public string ExpressImageUrl { get; set; }

        /// <summary>
        /// 微信发模板消息的 消息id
        /// </summary>
        public long Msgid { get; set; }

        public bool IsNeedAwardInfo { get; set; }

        /// <summary>
        /// 信息是否合法
        /// </summary>
        public bool IsInvalidated { get; set; }

        /// <summary>
        /// 核奖验证码，中奖用户知道的验证码
        /// </summary>
        public string SecurityCode { get; set; }

        /// <summary>
        /// 分享后浏览的次数
        /// </summary>
        public int ViewCount { get; set; }

        /// <summary>
        /// 分享后点赞的次数
        /// </summary>
        public int LikeCount { get; set; }

        public string Description { get; set; }

        public bool IsConfirmed { get; set; }

        public long Id { get; set; }
    }
}
