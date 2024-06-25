using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sensing.SDK
{
    public class DeviceType
    {
        public const string Pad = "Pad";
        public const string PC = "PC";
    }
    public class LoginDevice
    {
        public string DeviceType { get; set; }
        public int DeviceId { get; set; }
        public string Mac { get; set; }
        public string Ip { get; set; }
    }

    public class DeviceStatus
    {
        public int AreaId { get; set; }
        public int DeviceId { get; set; }
        public string Name { get; set; }
        public bool Online { get; set; } 
        public bool Lock { get; set; }
        public string DeviceName { get; set; }
    }
}