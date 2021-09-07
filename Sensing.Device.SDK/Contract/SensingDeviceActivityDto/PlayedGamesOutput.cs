using Sensing.SDK.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sensing.SDK.Contract.SensingDeviceActivityDto
{
    public class PlayedGamesOutput : EntityDto<long>
    {
        public string GameName { get; set; }
        public long Count { get; set; }
        public long GameId { get; set; }
        public long maxScore { get; set; }
    }
}
