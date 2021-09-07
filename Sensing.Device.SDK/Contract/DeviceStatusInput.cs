using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sensing.SDK.Contract
{
    public class DeviceStatusInput
    {
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public double Cpu { get; set; }
        public double Memory { get; set; }
    }
}
