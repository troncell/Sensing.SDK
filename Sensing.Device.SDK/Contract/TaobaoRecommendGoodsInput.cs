using System;
using System.Collections.Generic;
using System.Text;

namespace SensingStoreCloud.Devices.Dto.SensingDevice
{
    public class TaobaoRecommendGoodsInput
    {
        public string image_list_json { get; set; }
        public string collect_time { get; set; }
        public long flow_code { get; set; }
        public long store_id { get; set; }
        public string device_id { get; set; }

        public string response_type { get; set; }

        public string Params { get; set; }

        public string Subkey { get; set; }
    }
}
