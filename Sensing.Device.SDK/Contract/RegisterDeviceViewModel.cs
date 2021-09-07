using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sensing.SDK.Contract
{
    public class RegisterDeviceInput
    {
        public string Name { get; set; }

        public string IntranetIP { get; set; }

        public string InternetIP { get; set; }

        public string Mac { get; set; }

        public string Address { get; set; }

        public int DeviceTypeId { get; set; }

        public string LicenseInfo { get; set; }

        public string HardwareCode { get; set; }

        public double Resolution_Width { get; set; }

        public double Resolution_Height { get; set; }

        public string OS { get; set; }
    }
}
