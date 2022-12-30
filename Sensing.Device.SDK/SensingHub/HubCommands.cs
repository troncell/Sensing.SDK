using Sensing.SDK.Contract;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sensing.Device.SDK.SensingHub
{
    public class MessagePackageTypeConstants
    {
        public const string Notification = "Notification";
        public const string AreaLock = "AreaLock";
        public const string Command = "Command";
        public const string Auth = "Auth";
        public const string DeviceStatus = "DeviceStatus";
        public const string CommonSetting = "CommonSetting";
    }

    public class AreaDeviceCommandTypeSendConstants
    {
        public static string Lock = "Lock";
        public static string ForceLock = "ForceLock";
        public static string Release = "Release";
    }

    public class AreaDeviceCommandTypePushConstants
    {
        public static string LockedByOthers = "LockedByOthers";
        public static string KickOff = "KickOff";
        public static string LockedByMe = "LockedByMe";
        public static string BroadcastLock = "BroadcastLock";
    }

    public class AreaDeviceCommand
    {
        public string Type { get; set; }
        public int AreaDeviceId { get; set; }
        public string Uuid { get; set; }
    }

    public class PadCommandTypeConstants
    {
        public static string EnterMode = "EnterMode";

        public static string Play = "Play";

        public static string TurnOnLight = "TurnOnLight";
        public static string TurnOffLight = "TurnOffLight";

        public static string TurnOffDevice = "TurnOffDevice";

        public static string AutoPPT = "AutoPPT";
        public static string PausePPT = "PausePPT";
        public static string LastPPT = "LastPPT";
        public static string NextPPT = "NextPPT";

        public static string PlayVideo = "PlayVideo";
        public static string PauseVideo = "PauseVideo";

        public static string VolumeUp = "VolumeUp";
        public static string VolumeDown = "VolumeDown";
        public static string VolumeOff = "VolumeOff";

        public static string PlayList = "PlayList";
        public static string PlaySingle = "PlaySingle";

        public static string SyncResource = "SyncResource";
        public static string ClientSoftwareUpdate = "CheckPCUpdate";

        public static string ScreenDisplay = "ScreenDisplay";

        public static string ShutdownAll = "ShutdownAll";
        public static string ShowGreeting = "ShowGreeting";
        public static string ShowDefault = "ShowDefault";
    }

    public class PadCommand
    {
        public string Type { get; set; }
        public int AreaDeviceId { get; set; }
        public int DeviceId { get; set; }
        public int ModeId { get; set; }
        public int AreaId { get; set; }
        public int ProfileId { get; set; }
        public int ResourceId { get; set; }
    }

    public class AuthCommandTypeConstants
    {
        public const string PCLogin = "PCLogin";
        public const string PadLogin = "PadLogin";
        public const string DebugConsole = "DebugConsole";
    }

    public class AuthCommand
    {
        public string Type { get; set; }

        public int DeviceId { get; set; }

        public string Uuid { get; set; }
    }

    public class DeviceStatusCommandTypeConstants
    {
        public const string GetAllDevicesStatus = "GetAllDevicesStatus";
        public const string AllDevicesStatus = "AllDevicesStatus";
        public const string Online = "Online";
        public const string Offline = "Offline";
        public const string LockChanged = "LockChanged";
    }

    public class DeviceStatusCommand
    {
        public string Type { get; set; }
        public DeviceStatus DeviceStatus { get; set; }
        public List<DeviceStatus> AllDeviceStatus { get; set; }

    }

    public class CommonSettingCommand
    {
        public int PadTimeOut { get; set; }

        public DateTime AutoSync { get; set; }

        public int PptTimeOut { get; set; }
    }

    public class NotificationMessagePackage : MessagePackage<string>
    {
        public NotificationMessagePackage()
        {
            Type = MessagePackageTypeConstants.Notification;
        }
    }

    public class PadMessagePackage : MessagePackage<PadCommand>
    {
        public PadMessagePackage()
        {
            Type = MessagePackageTypeConstants.Command;
        }
    }

    public class AreaDevicePackage : MessagePackage<AreaDeviceCommand>
    {
        public AreaDevicePackage()
        {
            Type = MessagePackageTypeConstants.AreaLock;
        }
    }

    public class ClientAuthPackage : MessagePackage<AuthCommand>
    {
        public ClientAuthPackage()
        {
            Type = MessagePackageTypeConstants.Auth;
        }
    }

    public class DeviceStatusPackage : MessagePackage<DeviceStatusCommand>
    {
        public DeviceStatusPackage()
        {
            Type = MessagePackageTypeConstants.DeviceStatus;
        }
    }

    public class CommonSettingPackage : MessagePackage<CommonSettingCommand>
    {
        public CommonSettingPackage()
        {
            Type = MessagePackageTypeConstants.CommonSetting;
        }
    }

    public class MessagePackage<T>
    {
        public string Type { get; set; }
        public T Content { get; set; }
    }

    public class MessagePackage
    {
        public string Type { get; set; }
    }
}
