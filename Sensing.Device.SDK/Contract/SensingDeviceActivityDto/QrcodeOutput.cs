using System;
using System.Collections.Generic;
using System.Text;

namespace SensingStoreCloud.Activity
{
    public class QrcodeOutput
    {
        public string QrCodeUrl { get; set; }
        public string QrCodeImage { get; set; }
        public string QrCodeId { get; set; }

    }

    public class QrcodeActionOutput : QrcodeOutput
    {
        public long ActionId { get; set; }
    }

    public class SnsUserInfoOutput
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
        public EnumSnsType SnsType { get; set; }
    }

    public class UserActionInfoOutput
    {
        public int Id { get; set; }
        public string QrCodeId { get; set; }
        public double? Score { get; set; }
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
        public long? AwardId { get; set; }
        public string ExtensionData { get; set; }
        //public AwardOutput Award { get; set; }
        /// <summary>
        /// 用户id 特定公众号下面的user/说到Taobao的用户Id
        /// </summary>
        public long? SnsUserInfoId { get; set; }

        public virtual SnsUserInfoOutput SnsUserInfo { get; set; }

    }

    public class UserDataOutput
    {
        public Snsuserinfo SnsUserInfo { get; set; }
        public Activityuserdata ActivityUserData { get; set; }

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

        public class Activityuserdata
        {
            public int SnsUserInfoId { get; set; }
            public string Name { get; set; }
            public string Phone { get; set; }
            public string Address { get; set; }
            public string IdentityID { get; set; }
            public string CompanyName { get; set; }
            public bool IsSigned { get; set; }
            public bool IsValidated { get; set; }
            public bool IsRegistered { get; set; }
            public bool IsGamed { get; set; }
            public int Id { get; set; }
        }
    }



    public class ChatMessage
    {
        public int Id { get; set; }
        public long? SnsUserInfoId { get; set; }

        public virtual SnsUserInfoOutput SnsUserInfo { get; set; }
        public string Content { get; set; }
    }
}
