using SensingStoreCloud.Activity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sensing.SDK.Contract.Faces
{
    public class FaceDataInput : SensingDeviceGameInputBase
    {
        /// <summary>
        /// 
        /// </summary>
        public byte[] FaceBytes { get; set; }

        public string FaceUrl { get; set; }
    }


    public class UserFaceDataInput : FaceDataInput
    {
        public string OpenId { get; set; }
        public EnumSnsType SnsType { get; set; }
    }

    public class FaceRecordInput : PagedSortedAndFilteredInputDto
    {
        public string Gender { get; set; }
        public string CollectionStartTime { get; set; }
        public string CollectionEndTime { get; set; }
    }


    public class FaceRecordOutput
    {
        public int Id { get; set; }
        public string Action { get; set; }
        public string PageName { get; set; }
        public string FaceUrl { get; set; }
        public DateTime CollectionTime { get; set; }
        public string Gender { get; set; }
        public string Age { get; set; }
        public string Happpiness { get; set; }
        public string Emotion { get; set; }
        public string Score { get; set; }
    }


}
