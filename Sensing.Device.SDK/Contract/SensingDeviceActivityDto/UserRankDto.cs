using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sensing.SDK.Contract.SensingDeviceActivityDto
{

    public class UserRankDto
    {
        public int TotalCount { get; set; }
        public Item[] Items { get; set; }
    }

    public class Item
    {
        public string QrCodeId { get; set; }
        public int Score { get; set; }
        public string PostUrl { get; set; }
        public string gameImage { get; set; }
        public string playerImage { get; set; }
        public string playerPhone { get; set; }
        public string playerEmail { get; set; }
        public int playerAge { get; set; }
        public int shareCount { get; set; }
        public int viewCount { get; set; }
        public int likeCount { get; set; }
        public string scanQrCodeTime { get; set; }
        public bool isSigned { get; set; }
        public int awardId { get; set; }
        public bool isForged { get; set; }
        public string forgedReason { get; set; }
        public string type { get; set; }
        public string extensionData { get; set; }
        public int snsUserInfoId { get; set; }
        public Snsuserinfo snsUserInfo { get; set; }
        public DateTime creationTime { get; set; }
        public bool isSuccess { get; set; }
        public string failReason { get; set; }
        public int id { get; set; }
    }

    public class Snsuserinfo
    {
        public int id { get; set; }
        public string openid { get; set; }
        public string nickname { get; set; }
        public int sex { get; set; }
        public string language { get; set; }
        public string city { get; set; }
        public string country { get; set; }
        public string province { get; set; }
        public string headimgurl { get; set; }
        public string unionid { get; set; }
        public string remark { get; set; }
        public int weixinGroupid { get; set; }
        public string snsType { get; set; }
    }

}

