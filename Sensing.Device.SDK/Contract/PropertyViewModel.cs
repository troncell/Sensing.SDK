using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sensing.SDK.Contract
{
    public class PropertyViewModel
    {
        public long Id { get; set; }
        public string Name { get; set; }

        public bool IsForSale { get; set; } = false;

        public bool IsDefaultDecideImage { get; set; }

        public List<PropertyValueViewModel> PropertyValues;
        public string Description { get; set; }
    }

    public class PropertyValueViewModel
    {
        public long Id { get; set; }
        public string Value { get; set; }
        public string Description { get; set; }
        public string DefaultImage { get; set; }
    }
}
