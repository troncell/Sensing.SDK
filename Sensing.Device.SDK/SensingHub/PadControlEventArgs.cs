using Sensing.Device.SDK.SensingHub;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SensingHub.Sdk
{
    public class PadMessagePackageEventArgs
    {
        public PadMessagePackage Package { get; private set; }

        public PadMessagePackageEventArgs(PadMessagePackage package)
        {
            Package = package;
        }
    }

    public class DeviceStatusPackageEventArgs
    {
        public DeviceStatusPackage Package { get; private set; }

        public DeviceStatusPackageEventArgs(DeviceStatusPackage package)
        {
            Package = package;
        }
    }
}
