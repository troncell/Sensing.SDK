using System;
using System.Collections.Generic;
using System.Text;

namespace Sensing.SDK.Contract
{
    public class TenantAppSdkSetting
    {
        public string Alias { get; set; }

        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public string ExtensionData { get; set; }

        /// <summary>
        /// 用户自定义游戏素材zip路径
        /// </summary>
        public string MaterialPacketUrl { get; set; }

        /// <summary>
        /// 是否是默认启动的程序.
        /// </summary>
        public bool IsDefault { get; set; }
    }

    public class SoftwareSdkModel
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string Owner { get; set; }
        public string LogoUrl { get; set; }
        public string LargeImageUrl { get; set; }
        public string VersionNumber { get; set; }
        public string PackageUrl { get; set; }
        public string Type { get; set; }
        public string EnvType { get; set; }
        public string Description { get; set; }
        public string Language { get; set; }
        public string ExePath { get; set; }
        public string ExtensionData { get; set; }
        public string GameType { get; set; }
        public string Setting { get; set; }
        public string Url { get; set; }

        /// <summary>
        /// 最佳分辨率, 1920*1080.
        /// </summary>
        public int? TargetResolution_Width { get; set; }

        public int? TargetResolution_Height { get; set; }
    }

    public class DeviceSoftwareSdkModel
    {
        public long Id { get; set; }
        public long DeviceId { get; set; }
        public TenantAppSdkSetting TenantAppSetting { get; set; }
        public SoftwareSdkModel Software { get; set; }
        public bool IsDefault { get; set; }
        public string MaterialPacketUrl { get; set; }
        public string Alias { get; set; }
    }
}
