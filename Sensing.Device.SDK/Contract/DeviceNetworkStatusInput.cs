using System;

namespace Sensing.SDK.Contract
{
    public class DeviceNetworkStatusInput
    {
        public DateTime CollectionTime { get; set; }
        public DateTime CollectionEndTime { get; set; }
        public int PingSeed { get; set; }
    }
}
