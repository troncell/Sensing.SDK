using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sensing.SDK.Contract.SensingDeviceActivityDto
{
   public class PersonDataOutput
    {
        public int SnsUserInfoId { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string IdentityID { get; set; }
        public string CompanyName { get; set; }
        public bool IsSigned { get; set; }
        public bool IsValidated { get; set; }
        public bool IsRegistered { get; set; }
        public bool IsGamed { get; set; }
        public int Id { get; set; }
    }
}
