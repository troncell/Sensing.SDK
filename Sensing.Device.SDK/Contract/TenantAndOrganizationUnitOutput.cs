using System;
using System.Collections.Generic;
using System.Text;

namespace SensingStoreCloud.Devices.Dto
{
    public class TenantAndOrganizationUnitOutput
    {
        public int TenantId { get; set; }
        public string TenantName { get; set; }
        public long? OrganizationUnitId { get; set; }
        public string OrganizationUnitName { get; set; }
    }
}
