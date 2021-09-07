using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sensing.SDK.Contract
{
    public class LikeInfoViewModel
    {
        public long Id { get; set; }
        public int TenantId { get; set; }

        public long? OrganizationUnitId { get; set; }

        public string Code { get; set; }

        public int OrderNumber { get; set; }

        public ICollection<LikeItemViewModel> LikeItems { get; set; }

        public string Name { get; set; }

        public string Image { get; set; }

        public string Description { get; set; }
    }

    public class LikeItemViewModel
    {
        public long Id { get; set; }
        public long LikeInfoId { get; set; }
        public long SkuId { get; set; }
        public string SkuPicUrl { get; set; }
        public string SkuTitle { get; set; }
        public string Reason { get; set; }
    }
}
