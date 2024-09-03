using Sensing.SDK.Contract;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppPod.DataAccess.Models
{
    public class DeviceSetting
    {
        [PrimaryKey]
        public int Id { get; set; }

        public long DeivceId { get; set; }
        public string Name { get; set; }
        public string TenantName { get; set; }
        public long TenantId { get; set; }

        public int GroupId { get; set; }
        public string GroupName { get; set; }

        public string SubKey { get; set; }


        public string Mac { get; set; }

        public string IntranetIP { get; set; }



        public string Message { get; set; }

        public DateTime LastUploadedTime { get; set; }
        public DateTime CreatedTime { get; set; }

        public bool IsSuccessed { get; set; }

        public string OnlineTrafficTarget { get; set; }

        public string OS { get; set; }
    }
}
