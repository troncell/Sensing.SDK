using System;
using System.Collections.Generic;
using System.Text;

namespace Sensing.SDK.Contract
{
    public class ProductCategorySDKModel
    {
        public int Id { get; set; }

        /// <summary>
        /// 可被理解的分类唯一编码.
        /// </summary>
        public string CategoryCode { get; set; }
        /// <summary>
        /// 分类的名称
        /// </summary>
        public string Name { get; set; }


        public int ParentCategoryId { get; set; }

        /// <summary>
        /// 分类的图片
        /// </summary>
        public string ImageUrl { get; set; }


        public string IconUrl { get; set; }


        public bool IsLocal { get; set; }


        public bool IsSpecial { get; set; }

        public string FromType { get; set; }
        public List<int> Ids { get; set; }
    }
}
