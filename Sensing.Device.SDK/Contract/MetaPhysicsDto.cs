using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sensing.SDK.Contract
{
    public class MetaPhysicsDto 
    {
        public long Id { get; set; }
        public int? TenantId { get; set; }

        /// <summary>
        /// Zodiac,Astro
        /// </summary>
        public long TypeId { get; set; }

        public MetaphysicsTypeDto Type { get; set; }

        public string Name { get; set; }

        /// <summary>
        /// 别名
        /// </summary>
        public string AliasName { get; set; }

        /// <summary>
        /// Astro, 取月和日，年忽略.
        /// Zodiac,暂时忽略.
        /// </summary>
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public string LogoUrl { get; set; }
        /// <summary>
        /// 对应玄学的图片.
        /// </summary>
        public string PicUrl { get; set; }

        public string Description { get; set; }

        public string From { get; set; }
        public string FromLogoUrl { get; set; }
        public string ThemeColor { get; set; }
        public string OuterId { get; set; }
    }

    public class MetaphysicsTypeDto
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public int? TenantId { get; set; }
        public string Description { get; set; }
    }
}
