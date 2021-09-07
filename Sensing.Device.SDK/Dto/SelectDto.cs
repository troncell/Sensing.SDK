using System;
using System.Collections.Generic;
using System.Text;

namespace SensingStoreCloud.Dto
{
    public class SelectDto<T>
    {
        public T SelectKey { get; set; }
        public string SelectValue { get; set; }
    }
}
