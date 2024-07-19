using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SensingHub.Sdk
{
    public class RemoteCastEventArgs
    {
        public string FileName { get; private set; }
        public string DeviceId { get; private set; }

        public RemoteCastEventArgs(string fileName, string deviceId)
        {
            FileName = fileName;
            DeviceId = deviceId;
        }
    }

    public class StopRemoteCastEventArgs
    {
        public string DeviceId { get; private set; }

        public StopRemoteCastEventArgs(string deviceId)
        {
            DeviceId = deviceId;
        }
    }
}
