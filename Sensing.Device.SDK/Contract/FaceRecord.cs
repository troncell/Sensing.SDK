using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sensing.SDK.Contract
{
    public class FaceRecord
    {
        public string Face { get; set; }
        public DateTime CollectionTime { get; set; }
        public DateTime CollectEndTime { get; set; }
        public string SoftwareName { get; set; }
        public int SoftwareId { get; set; }
        public string Comments { get; set; }
        public string PageName { get; set; }
        public string PreviousPageName { get; set; }
        public string PreviousPageArea { get; set; }

    }


}
