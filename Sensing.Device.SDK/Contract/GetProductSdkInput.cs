
using SensingStoreCloud.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sensing.SDK.Contract
{
    public class GetSensingSdkCommonInput : PagedSortedAndFilteredInputDto
    {
        public string Subkey { get; set; }
    }
}
