using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;

namespace Sensing.SDK.Contract.SensingDeviceActivityDto
{


    public class GameUserActionOutput
    {
        public int Id { get; set; }
        public string Nickname { get; set; }
        public string Headimgurl { get; set; }
        public string ExtensionData { get; set; }
    }
}
