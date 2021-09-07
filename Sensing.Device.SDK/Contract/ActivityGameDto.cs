using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sensing.SDK.Contract
{
    public class ActivityGameDto
    {
        public long Id { get; set; }

        //ActivityGame 别名
        public string Name { get; set; }
        /// <summary>
        /// 某些特定活动扫码前需要做一些check
        /// </summary>
        public bool IsNeedCheckBeforScanQRCode { get; set; }
        /// <summary>
        /// 该活动中游戏最大支持人数
        /// </summary>
        public int SupportedPlayersCount { get; set; }

        //Software/Game名称 不可修改
        public string SoftwareName { get; set; }
        public int GameType { get; set; }

        /// <summary>
        /// 游戏密钥
        /// </summary>
        public string SecurityKey { get; set; }

        public bool IsSendWechatBeforeGame { get; set; }
        public bool IsSendWechatAfterGame { get; set; }
        public bool IsShareAction { get; set; }

        public int TenantId { get; set; }
        public long SoftwareId { get; set; }
        public long DispatchedSoftwareId { get; set; }
        public long ActivityId { get; set; }

        /// <summary>
        /// 游戏后的分享设置
        /// </summary>
        public long? ActionShareId { get; set; }


        /// <summary>
        /// 是否在游戏前发送消息
        /// 比如有些游戏需要先扫码才能玩，一旦用户扫码，用户的微信会接收到一条消息，这个时候就是游戏前发送消息
        /// </summary>
        public bool IsSendSnsMsgBeforeGame { get; set; }

        public long? AfterGameMessageId { get; set; }

        /// <summary>
        /// 是否在游戏后发送消息
        /// 用户玩完游戏后，系统会推送一些游戏成绩到用户微信里，决定是否发这样的消息
        /// </summary>
        public bool IsSendSnsMsgAfterGame { get; set; }

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

        public long? BeforeGameMessageId { get; set; }

        public string ExtensionData { get; set; }
    }
}
