using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sensing.SDK.Contract
{
    public class BrandDto
    {
        public long Id { get; set; }
        public string Code { get; set; }

        public virtual int OrderNumber { get; set; }
        
        public string Name { get; set; }

        /// <summary>
        /// 品牌 Logo
        /// </summary>
        public string LogoUrl { get; set; }

        /// <summary>
        /// 品牌大图
        /// </summary>
        public string ImageUrl { get; set; }

        /// <summary>
        /// 状态
        /// </summary>

        public string State { get; set; }

        /// <summary>
        /// 品牌主题色   16进制编码：#FFFFFF
        /// </summary>
        public string MainColor { get; set; }

        public string Description { get; set; }

        /// <summary>
        /// 管理的外部资源
        /// </summary>
        public virtual ICollection<EntityFileSdkModel> ItemImagesOrVideos { get; set; }

        public string Slogan { get; set; }
    }
    public class BrandResourceFileDto
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string FileUrl { get; set; }
        public string FromType { get; set; }
        public string ResourType { get; set; }
        public string Content { get; set; }
        public string Type { get; set; }
        public string Usage { get; set; }
    }
}
