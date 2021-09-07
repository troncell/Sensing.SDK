using System;
using System.Collections.Generic;
using System.Text;

namespace Sensing.SDK.Contract
{
    public class StaffSdkModel
    {
        public long Id { get; set; }
        public string PhoneNumber { get; set; }
        public string EmailAddress { get; set; }
        public string UserName { get; set; }
        public string FullName { get; set; }
        public DateTime? LastLoginTime { get; set; }
        
        public string OuterId { get; set; }

        public bool CanApiAccess { get; set; }

        public bool IsStaff { get; set; }

        /// <summary>
        /// male,female.
        /// </summary>
        public string Gender { get; set; }

        public string RfidCode { get; set; }

        public IEnumerable<OnlineStoreProfileSdkModel> OnlineStoreInfos { get; set; }
    }

    public class OnlineStoreProfileSdkModel
    {
        public string OnlineStoreName { get; set; }

        public string Code { get; set; }
    }
}
