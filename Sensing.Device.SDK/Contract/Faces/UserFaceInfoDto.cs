using SensingStoreCloud.Activity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sensing.SDK.Contract.Faces
{
    public class UserFaceInfoDto
    {
        public long? SnsUserInfoId { get; set; }
        public SnsUserInfoDto SnsUserInfo { get; set; }

        /// <summary>
        /// 客户的人脸照片
        /// </summary>
        public string ImageUrl { get; set; }

        /// <summary>
        /// 人脸算法供应商
        /// </summary>
        public string Vendor { get; set; }

        /// <summary>
        /// 所属租户Id
        /// </summary>
        public int TenantId { get; set; }

        /// <summary>
        /// 人脸的扩展JSON字段
        /// </summary>
        public string ExtensionData { get; set; }

        /// <summary>
        /// 性别
        /// </summary>
        public string Gender { get; set; }

        /// <summary>
        /// 年龄
        /// </summary>
        public double Age { get; set; }

        /// <summary>
        /// 开心度
        /// </summary>
        public string Happpiness { get; set; }

        /// <summary>
        /// 表情
        /// </summary>
        public string Emotion { get; set; }
        /// <summary>
        /// 颜值
        /// </summary>
        public string Score { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public long? ParentUserId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string GroupName { get; set; }

        public string Name { get; set; }
    }

    public class SnsUserInfoDto
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
        /// 公众号的AppID/或者淘宝的TaobaoSellerId.
        /// </summary>
        public string SnsAppID { get; set; }


        /// <summary>
        /// 是否关注
        /// 0:不再关注
        /// 1：关注
        /// </summary>
        public int? Subscribe { get; set; }

        /// <summary>
        /// 当前公众号下面的openID,如果是淘宝,就存混淆nick
        /// </summary>
        public string Openid { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Nickname { get; set; }

        public int? Sex { get; set; }

        public string Language { get; set; }

        public string City { get; set; }
        public string Province { get; set; }
        public string Country { get; set; }

        public string Headimgurl { get; set; }

        /// <summary>
        /// 关注时间
        /// </summary>
        public DateTime? SubscribeTime { get; set; }

        /// <summary>
        ///     取消关注时间
        /// </summary>
        public DateTime? UnSubScribeTime { get; set; }

        /// <summary>
        /// 每个平台下用户的唯一Id.
        /// </summary>
        public string Unionid { get; set; }

        public string Remark { get; set; }

        public int? WeixinGroupid { get; set; }

        /// <summary>
        /// 是否成为粉丝
        /// </summary>
        public bool IsBecomeFans { get; set; }

        /// <summary>
        /// 用户真是姓名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 用户电话
        /// </summary>
        public string Phone { get; set; }

        /// <summary>
        /// 用户身份证号码
        /// </summary>
        public string IdentityID { get; set; }

        public EnumSnsType SnsType { get; set; }

        /// <summary>
        /// 是否是人脸会员
        /// </summary>
        public bool IsFaceMember { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string FaceMemberId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string FaceUrl { get; set; }
    }
}
