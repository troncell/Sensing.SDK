using Sensing.SDK.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppPod.DataAccess.Models
{
    public class PropertyValueInfo
    {
        public string Name { get; set; }
        public string ImageUrl { get; set; }
        public SkuSdkModel Sku { get; set; }
    }

    public class PropertyInfo
    {
        public PropertyInfo()
        {
            Values = new List<PropertyValueInfo>();
        }
        public string Name { get; set; }
        public bool IsKey { get; set; }
        public List<PropertyValueInfo> Values { get; set; }
    }
}
