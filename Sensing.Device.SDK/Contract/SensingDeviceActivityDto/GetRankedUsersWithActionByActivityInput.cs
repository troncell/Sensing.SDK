using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sensing.SDK.Contract.SensingDeviceActivityDto
{
    public class GetRankedUsersWithActionByActivityInput
    {
        [Required]
        public string RankColumn { get; set; }

        public DateTime? StartTime { get; set; }

        public DateTime? EndTime { get; set; }

        public bool? IsGameLevel { get; set; } = true;

        [Required]
        public string SecurityKey { get; set; }

        public string SoftwareCode { get; set; }

        public string ClientId { get; set; }



    }
}
