using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sensing.SDK
{
    public class DeviceProfile
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public IList<int> ResourceIds { get; set; }
    }
}