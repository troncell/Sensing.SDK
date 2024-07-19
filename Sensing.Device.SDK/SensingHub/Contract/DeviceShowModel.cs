using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sensing.SDK
{
    public class DeviceShowModel
    {
        public int Id { get; set; }
        public int MShowModelId { get; set; }

        public string Name { get; set; }
        public bool IsDefaultModel { get; set; }
        public string Arrangement { get; set; }
        public IEnumerable<DeviceShowArea> ShowAreas { get; set; }
    }
}