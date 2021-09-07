using Sensing.SDK.Contract;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SensingStoreCloud.Activity
{
    public enum ActionStatus
    {
        Start,
        Waitting,
        Playing,
        Done,
        AwardEdit,
        //未中奖，因为奖品已发完
        NotGetPrizeWithNoAward,
        //未中奖
        NotGetPrize
    }

    public enum EnumSnsType
    {
        Taobao,
        WeChat,
        Others,
    }

    public enum EnumQRStatus
    {
        /// <summary>
        /// 玩游戏之前生成此二维码
        /// </summary>
        [Display(Name = "游戏前扫码")]
        BeforeGame = 1,

        /// <summary>
        /// 玩游戏之后生成
        /// </summary>
        [Display(Name = "游戏后扫码")]
        AfterGame = 2,

        [Display(Name = "活动主页")]
        ActivityIntroduction,

        [Display(Name = "活动注册")]
        ActivityRegister,

        [Display(Name = "在线游戏")]
        OnlineGame,

        [Display(Name = "游戏结果")]
        GameResult,

        [Display(Name = "中奖主页")]
        Award
    }

    public interface ISensingDeviceGameInput
    {
        string SecurityKey { get; set; }
    }
    public class SensingDeviceGameInputBase : ISensingDeviceGameInput
    {
        [Required]
        public string SecurityKey { get; set; }

        public string Params { get; set; }
    }

    public class SensingDeviceGameListInputBase : PagedSortedAndFilteredInputDto, ISensingDeviceGameInput
    {
        [Required]
        public string SecurityKey { get; set; }
        public string Params { get; set; }
    }

    public class SensingDeviceGamePagedSortedFilteredInputBase : PagedSortedAndFilteredInputDto, ISensingDeviceGameInput
    {
        [Required]
        public string SecurityKey { get; set; }

        public string SoftwareCode { get; set; }

        public string ClientId { get; set; }

        //public void Normalize()
        //{
        //    if (string.IsNullOrEmpty(Sorting))
        //    {
        //        Sorting = "LastModificationTime";
        //    }
        //}
    }

    public class ActivityGamePageInput : SensingDeviceGamePagedSortedFilteredInputBase
    {

    }

    public class ActivityGameActionInput : SensingDeviceGamePagedSortedFilteredInputBase
    {
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
    }

    public class SnsUserTextDataInput : SensingDeviceGameInputBase
    {
        [Required]
        public string Text { get; set; }
        [Required]
        public string OpenId { get; set; }
        public EnumSnsType SnsType { get; set; }
    }


    public class Qrcode4LoginInput : SensingDeviceGameInputBase
    {
        public bool? IsSendWeChatMsg { get; set; }
        public string Params { get; set; }
        public EnumQRStatus QrType { get; set; }
        public EnumSnsType SnsType { get; set; }
        public string TargetUrl { get; set; }
    }

    public class CreatQrcodeInput
    {
        public string SecurityKey { get; set; }
        public bool IsSendWeChatMsg { get; set; }
        public string Params { get; set; }
        public string QrType { get; set; }
        public string SnsType { get; set; }
        public string TargetUrl { get; set; }
        public bool IsTransferred { get; set; }
    }


    public class Qrcode4UsersInput : SensingDeviceGameListInputBase
    {
        [Required]
        [Range(0, double.MaxValue)]
        public long QrcodeId { get; set; }

        //public void Normalize()
        //{
        //    if (string.IsNullOrEmpty(Sorting))
        //    {
        //        Sorting = "LastModificationTime";
        //    }
        //}
    }

    public class PersonDataInput 
    {
        public string SecurityKey { get; set; }
        public string OpenId { get; set; }
        public EnumSnsType SnsType { get; set; }
        public long ActionId { get; set; }
        public string Params { get; set; }
    }

    public class DataUsersInput
    {
        public bool IsGameLevel { get; set; }
        public string SecurityKey { get; set; }
        public string SoftwareCode { get; set;}
        public string ClientId { get; set; }
        public string Filter { get; set; }
        public string Sorting { get; set; }
        public long MaxResultCount { get; set; }
        public long SkipCount { get; set; }


    }

    public class PlayerActionDataSimpleDataInput : SensingDeviceGameListInputBase
    {
        [Required]
        [Range(0, double.MaxValue)]
        public long ActionId { get; set; }
        public string PlayerImage { get; set; }
        public string PlayingImage { get; set; }
        public double? Score { get; set; }
        public EnumQRStatus QrType { get; set; }
    }


    public class PlayerDataInput : Qrcode4LoginInput
    {
        public string PlayerImage { get; set; }
        public string PlayingImage { get; set; }
        public double? Score { get; set; }
        ////action的类型，应用自己确定(SkinTest_XiaoFu)
        public string Type { get; set; }
        //扩展的字段，一般json格式
        public string ExtensionData { get; set; }
        public bool IsTransferred { get; set; }
        //public ActionStatus? Status { get; set; }

    }

    public class PlayerActionDataInput : PlayerDataInput
    {
        [Required]
        [Range(0, double.MaxValue)]
        public long ActionId { get; set; }
    }

    public class PlayerDataActionInput : PlayerDataInput
    {
        public long ActionId { get; set; }
        public string OpenId { get; set; }
    }


    public class SnsUserActionDataInput : PlayerActionDataInput
    {
        [Required]
        [Range(0, double.MaxValue)]
        public string OpenId { get; set; }
        public string AvatarUrl { get; set; }
    }

    public class FaceUserFormDataInput : Qrcode4LoginInput
    {
        [Required]
        [Range(0, double.MaxValue)]
        public string OpenId { get; set; }
        public string AvatarUrl { get; set; }
        public string HeaderImage { get; set; }
        public string Phone { get; set; }
        public string Name { get; set; }
    }


    public class FaceUserDataInput : Qrcode4LoginInput
    {
        [Required]
        [Range(0, double.MaxValue)]
        public string OpenId { get; set; }
        public string AvatarUrl { get; set; }
        public string FaceUrl { get; set; }
        public string Phone { get; set; }
        public string Name { get; set; }
    }

    public class ActionDataInput : SensingDeviceGameInputBase
    {
        [Required]
        [Range(0, double.MaxValue)]
        public long ActionId { get; set; }
    }

    public class ActionInput
    {
        public long ActionId { get; set; }
    }

    public class PlayGameInput
    {
        public string OpenId { get; set; }
        public EnumSnsType SnsType { get; set; }
        public string Security { get; set; }
        public string Params { get; set; }
        public string Filter { get; set; }
        public string Sorting { get; set; }
        public long MaxResultCount { get; set;}
        public long SkipCount { get; set; }
    }

    public class AwardInput
    {
        public string SecurityKey { get; set; }
        public string Sorting { get; set; }
        public long MaxResultCount { get; set; }
    }


    public class AwardDataInput : SensingDeviceGameInputBase
    {
        [Required]
        [Range(0, double.MaxValue)]
        public long AwardId { get; set; }
        public bool IsNeedSendNotify { get; set;}
    }

    public class NotifyInput 
    {
        public long UserAwardId { get; set; }
        public string SecurityKey { get; set; }
        public string Params { get; set; }
    }



    public class UserAwardDataInput : SensingDeviceGameInputBase
    {
        [Required]
        [Range(0, double.MaxValue)]
        public long UserAwardId { get; set; }
    }
}
