using Sensing.SDK.Contract;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SensingStoreCloud.Activity
{
    public class ActionDataOutput
    {
        public Deviceactivitygame DeviceActivityGame { get; set; }
        public Device Device { get; set; }
        public string ExternalAccessSession { get; set; }
        public string TransferActionUrl { get; set; }
        public Activityflow[] ActivityFlows { get; set; }
        public string QrCodeId { get; set; }
        public int Score { get; set; }
        public string PostUrl { get; set; }
        public string GameImage { get; set; }
        public string PlayerImage { get; set; }
        public string PlayerPhone { get; set; }
        public string PlayerEmail { get; set; }
        public int PlayerAge { get; set; }
        public int ShareCount { get; set; }
        public int ViewCount { get; set; }
        public int LikeCount { get; set; }
        public string ScanQrCodeTime { get; set; }
        public bool IsSigned { get; set; }
        public int AwardId { get; set; }
        public bool IsForged { get; set; }
        public string ForgedReason { get; set; }
        public string Type { get; set; }
        public string ExtensionData { get; set; }
        public int SnsUserInfoId { get; set; }
        public Snsuserinfo SnsUserInfo { get; set; }
        public DateTime CreationTime { get; set; }
        public int Id { get; set; }
    }

    public class Deviceactivitygame
    {
        public int TenantId { get; set; }
        public string Name { get; set; }
        public int SoftwareId { get; set; }
        public Software Software { get; set; }
        public int DispatchedSoftwareId { get; set; }
        public int DeviceId { get; set; }
        public int ActivityId { get; set; }
        public Activity Activity { get; set; }
        public bool IsShareAction { get; set; }
        public int DispatchedActivityId { get; set; }
        public int SupportedPlayersCount { get; set; }
        public string OnlineBackground { get; set; }
        public string OnlineForeground { get; set; }
        public string OfflineBackground { get; set; }
        public string OfflineForeground { get; set; }
        public string MaterialPacketUrl { get; set; }
        public int MaxPlayCountPerUser { get; set; }
        public int MinPlayTimeDiff { get; set; }
        public bool IsGameStarted { get; set; }
        public DateTime LastGameOverTime { get; set; }
        public bool IsNeedCheckBeforScanQRCode { get; set; }
        public string SecurityKey { get; set; }
        public string WeChatAuthorizationType { get; set; }
        public string TaobaoAuthorizationType { get; set; }
        public int ActionShareId { get; set; }
        public Actionshare ActionShare { get; set; }
        public string ExtensionData { get; set; }
        public int Id { get; set; }
    }

    public class Software
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public string Owner { get; set; }
        public string Contact { get; set; }
        public int TenantId { get; set; }
        public string Url { get; set; }
        public string LogoUrl { get; set; }
        public string LargeImageUrl { get; set; }
        public int EnvType { get; set; }
        public string VersionNumber { get; set; }
        public int Type { get; set; }
        public int TargetResolution_Width { get; set; }
        public int TargetResolution_Height { get; set; }
        public string Language { get; set; }
        public int GameType { get; set; }
        public string Description { get; set; }
        public string ExtensionData { get; set; }
        public string Setting { get; set; }
        public int SupportedPlayersCount { get; set; }
        public bool IsSupportAsync { get; set; }
        public bool IsSendWechatBeforeGame { get; set; }
        public string SendWeChatBeforGameTabName { get; set; }
        public bool IsSendWechatAfterGame { get; set; }
        public string SendWeChatAfterGameTabName { get; set; }
        public bool IsShareAction { get; set; }
        public string ShareActionTabName { get; set; }
        public int Id { get; set; }
    }

    public class Activity
    {
        public int TenantId { get; set; }
        public int OrganizationUnitId { get; set; }
        public string Name { get; set; }
        public string ImagePath { get; set; }
        public string Address { get; set; }
        public string ActivityUrl { get; set; }
        public string ActivityLogoPath { get; set; }
        public bool IsSendAwardMessage { get; set; }
        public int BeforeWeChatMessageId { get; set; }
        public DateTime OpenDate { get; set; }
        public DateTime EndDate { get; set; }
        public int ViewCount { get; set; }
        public int LikeCount { get; set; }
        public int ActivityShareId { get; set; }
        public string WeChatAppID { get; set; }
        public int WeixinPublicAccountInfoId { get; set; }
        public string TaobaoSellerID { get; set; }
        public int ExternalAccessTokenInfoId { get; set; }
        public bool IsEnableWhiteUser { get; set; }
        public bool IsEnableBlackUser { get; set; }
        public string Organizer { get; set; }
        public bool IsAllowedRepeatAward { get; set; }
        public bool IsNeedAwardInfo { get; set; }
        public bool IsNeedValidate { get; set; }
        public string Rules { get; set; }
        public DateTime ActivityExcutedDate { get; set; }
        public bool IsTemplate { get; set; }
        public string IconPath { get; set; }
        public string Description { get; set; }
        public bool IsNeedRegister { get; set; }
        public string OwnerName { get; set; }
        public bool IsPublic { get; set; }
        public string Contactor { get; set; }
        public string OuterId { get; set; }
        public string ContactorPhone { get; set; }
        public string AuditStatus { get; set; }
        public Activityshare ActivityShare { get; set; }
    }

    public class Activityshare
    {
        public string Title { get; set; }
        public string Link { get; set; }
        public string ImageLink { get; set; }
        public string Description { get; set; }
        public string ActivityShareType { get; set; }
        public int Id { get; set; }
    }

    public class Actionshare
    {
        public string Title { get; set; }
        public string Link { get; set; }
        public string ImageLink { get; set; }
        public string Description { get; set; }
        public string ActivityShareType { get; set; }
        public int Id { get; set; }
    }

    public class Device
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Mac { get; set; }
        public string Os { get; set; }
        public string OnlineTrafficTarget { get; set; }
        public string OuterId { get; set; }
        public string OrganizationUnitName { get; set; }
        public int OrganizationUnitId { get; set; }
        public string OrganizationOuterId { get; set; }
        public string OutType { get; set; }
        public string TaobaoDeviceId { get; set; }
        public string SubKey { get; set; }
    }

    public class Snsuserinfo
    {
        public int Id { get; set; }
        public string Openid { get; set; }
        public string Nickname { get; set; }
        public int Sex { get; set; }
        public string Language { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string Province { get; set; }
        public string Headimgurl { get; set; }
        public string Unionid { get; set; }
        public string Remark { get; set; }
        public int WeixinGroupid { get; set; }
        public string SnsType { get; set; }
    }

    public class Activityflow
    {
        public string FlowUrl { get; set; }
        public string FlowType { get; set; }
        public bool IsUseOutside { get; set; }
        public int HtmlTemplateId { get; set; }
        public Htmltemplate HtmlTemplate { get; set; }
        public string OutsideLink { get; set; }
        public bool IsHasIntroduce { get; set; }
        public bool IsHasRegister { get; set; }
        public bool IsRegisterInCurrPage { get; set; }
        public bool IsHasGame { get; set; }
        public bool IsHasGameResult { get; set; }
        public bool IsHasReward { get; set; }
        public bool IsHasVote { get; set; }
        public string InsideHtml { get; set; }
        public int Id { get; set; }
    }

    public class Htmltemplate
    {
        public string Thumbnail { get; set; }
        public string BigImage { get; set; }
        public string ActionUrl { get; set; }
        public string Args { get; set; }
        public string Title { get; set; }
        public string Logo { get; set; }
        public string BackgroundImage { get; set; }
        public string TemplateType { get; set; }
        public string Description { get; set; }
        public int Id { get; set; }
    }
}
