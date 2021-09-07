using Sensing.SDK.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace SensingStoreCloud.Activity
{
    public enum SnsActivityAuthorizationType
    {
        Follow,
        Web,
        None
    }

    public class DeviceActivityGameOutput
    {
        public int TenantId { get; set; }
        /// <summary>
        /// 活动游戏名称.
        /// </summary>
        public string Name { get; set; }
        public long SoftwareId { get; set; }
        public SoftwareOutput Software { get; set; }
        public long DispatchedSoftwareId { get; set; }

        public long? DeviceId { get; set; }
        //public DispatchedSoftware DispatchedSoftware { get; set; }
        public long ActivityId { get; set; }
        public virtual ActivityOutput Activity { get; set; }
        public bool IsShareAction { get; set; }

        public long? DispatchedActivityId { get; set; }

        /// <summary>
        /// 该活动中游戏最大支持人数
        /// </summary>
        public int SupportedPlayersCount { get; set; }

        /// <summary>
        /// 线上模板的背景图片
        /// </summary>
        public string OnlineBackground { get; set; }

        /// <summary>
        /// 线上模板的前景图片
        /// </summary>
        public string OnlineForeground { get; set; }

        /// <summary>
        /// 线下游戏的背景图片
        /// </summary>
        public string OfflineBackground { get; set; }

        /// <summary>
        /// 线下游戏前景图片
        /// </summary>
        public string OfflineForeground { get; set; }

        /// <summary>
        /// 用户自定义游戏素材zip路径
        /// </summary>
        public string MaterialPacketUrl { get; set; }

        /// <summary>
        /// 一个用户多长时间内最多玩几次
        /// </summary>
        public int MaxPlayCountPerUser { get; set; }
        public int MinPlayTimeDiff { get; set; }

        /// <summary>
        /// 活动关联的游戏是否已经开始
        /// </summary>
        public bool IsGameStarted { get; set; }

        public DateTime? LastGameOverTime { get; set; }

        /// <summary>
        /// 某些特定活动扫码前需要做一些check
        /// </summary>
        public bool IsNeedCheckBeforScanQRCode { get; set; }

        /// <summary>
        /// 用户直接访问特定活动游戏
        /// </summary>
        public string SecurityKey { get; set; }

        public SnsActivityAuthorizationType WeChatAuthorizationType { get; set; }

        public SnsActivityAuthorizationType TaobaoAuthorizationType { get; set; }
    }
    public class ActivityOutput
    {
        /// <summary>
        /// 所属的租户 Id
        /// </summary>
        public int TenantId { get; set; }

        /// <summary>
        /// 所属的组织 Id
        /// </summary>
        public long? OrganizationUnitId { get; set; }

        /// <summary>
        /// 活动名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 活动图片
        /// </summary>
        public string ImagePath { get; set; }

        /// <summary>
        /// 活动线下地址
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// 活动的首页url
        /// </summary>
        public string ActivityUrl { get; set; }

        /// <summary>
        /// 活动Logo的url
        /// </summary>
        public string ActivityLogoPath { get; set; }

        /// <summary>
        /// 是否发送中奖消息
        /// </summary>
        public bool IsSendAwardMessage { get; set; }

        public long? BeforeWeChatMessageId { get; set; }

        /// <summary>
        /// 活动开始时间
        /// </summary>
        public DateTime? OpenDate { get; set; }

        /// <summary>
        /// 活动结束时间
        /// </summary>
        public DateTime? EndDate { get; set; }

        /// <summary>
        /// 分享后浏览的次数（活动在我们平台的）
        /// </summary>
        public int ViewCount { get; set; }

        /// <summary>
        /// 分享后点赞的次数（活动在我们平台的）
        /// </summary>
        public int LikeCount { get; set; }

        public long? ActivityShareId { get; set; }

        //public virtual ActivityShare ActivityShare { get; set; }

        /// <summary>
        /// 公众号AppId
        /// </summary>
        public string WeChatAppID { get; set; }

        /// <summary>
        /// Taobao卖家的Id
        /// </summary>
        public string TaobaoSellerID { get; set; }

        /// <summary>
        /// 是否启用白名单
        /// </summary>
        public bool IsEnableWhiteUser { get; set; }

        ///// <summary>
        ///// 活动模板
        ///// </summary>
        //[Display(Name = nameof(rs.ActivityHtmlTemplateID), ResourceType = typeof(rs))]
        //public int? ActivityHtmlTemplateID { get; set; }

        //[Display(Name = nameof(rs.ActivityHtmlTemplate), ResourceType = typeof(rs))]
        //public virtual HtmlTemplate ActivityHtmlTemplate { get; set; }

        /// <summary>
        /// 活动主办方
        /// </summary>
        public string Organizer { get; set; }

        /// <summary>
        /// 是否允许某一个人重复中奖
        /// </summary>
        public bool IsAllowedRepeatAward { get; set; }

        /// <summary>
        /// 是否需要填写中奖信息，如地址姓名电话等。
        /// </summary>
        public bool IsNeedAwardInfo { get; set; }

        /// <summary>
        /// 活动规则
        /// </summary>
        public string Rules { get; set; }

        /// <summary>
        /// 活动执行时间
        /// </summary>
        public DateTime? ActivityExcutedDate { get; set; }

        /// <summary>
        /// 该活动是否是模板
        /// </summary>
        public bool IsTemplate { get; set; }

        /// <summary>
        /// 图标路径 更多用处是在创建活动选模板的时候
        /// </summary>
        public string IconPath { get; set; }

        /// <summary>
        /// 活动描述
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 次活动需要注册，
        /// todo:william,可以考虑ActivityGame里面增加这个变量.
        /// </summary>
        public bool IsNeedRegister { get; set; }

        /// <summary>
        /// 活动的所有者
        /// </summary>
        public string OwnerName { get; set; }

        /// <summary>
        /// 表明此活动是否是正式使用，或者是试用.
        /// </summary>
        public bool IsPublic { get; set; }

        /// <summary>
        /// 活动联系人
        /// </summary>
        public string Contactor { get; set; }

        public string OuterId { get; set; }

        /// <summary>
        /// 活动联系人电话
        /// </summary>
        public string ContactorPhone { get; set; }
    }

    public class SoftwareOutput : EntityDto<long>
    {
        /// <summary>
        /// 软件或软件产品的名称.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 软件唯一标号，便于部署的名字.
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 软件开发的负责人.
        /// </summary>
        public string Owner { get; set; }

        public string Contact { get; set; }

        public int? TenantId { get; set; }

        /// <summary>
        /// 在线游戏链接地址
        /// OnLine时使用该地址,Offline的h5也使用该地址.
        /// </summary>
        public string Url { get; set; }


        public string LogoUrl { get; set; }

        public string LargeImageUrl { get; set; }

        public int EnvType { get; set; }

        /// <summary>
        /// 版本号,格式为 1.0.0.5
        /// </summary>
        public string VersionNumber { get; set; }

        public int Type { get; set; }


        /// <summary>
        /// 最佳分辨率, 1920*1080.
        /// </summary>
        public int? TargetResolution_Width { get; set; }

        public int? TargetResolution_Height { get; set; }

        public string Language { get; set; }

        public int GameType { get; set; }

        public string Description { get; set; }

        #region SNS Settings.

        /// <summary>
        /// 游戏支持的同时玩游戏人数
        /// </summary>
        public int? SupportedPlayersCount { get; set; }

        /// <summary>
        /// 是否支持同步
        /// </summary>
        public bool IsSupportAsync { get; set; } = false;

        /// <summary>
        /// 是否在游戏前发送消息
        /// 比如有些游戏需要先扫码才能玩，一旦用户扫码，用户的微信会接收到一条消息，这个时候就是游戏前发送消息
        /// </summary>
        public bool IsSendWechatBeforeGame { get; set; }
        /// <summary>
        /// 表示这个消息什么场景会发，比如游戏前，或者扫码时
        /// </summary>
        public string SendWeChatBeforGameTabName { get; set; }
        /// <summary>
        /// 是否在游戏后发送消息
        /// 用户玩完游戏后，系统会推送一些游戏成绩到用户微信里，决定是否发这样的消息
        /// </summary>
        public bool IsSendWechatAfterGame { get; set; }
        /// <summary>
        /// 表示这个消息什么场景会发，比如游戏前，或者扫码时
        /// </summary>
        public string SendWeChatAfterGameTabName { get; set; }
        /// <summary>
        /// 游戏是否支持分享
        /// </summary>
        public bool IsShareAction { get; set; } = true;
        /// <summary>
        /// 表示分享tab的名称
        /// </summary>
        public string ShareActionTabName { get; set; } = "游戏分享";

        #endregion
    }
}