using Sensing.SDK.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppPod.DataAccess.Models
{
    public enum ProductType
    {
        Product,
        Sku
    }
    public class ShowProductInfo
    {
        public long Id { get; set; }
        public ProductType Type { get; set; }
        public string Name { get; set; }
        public string ProductName { get; set; }
        public long Quantity { get; set; }
        public double Price { get; set; }
        public double PromPrice { get; set; }
        public string ImageUrl { get; set; }
        public string TagIconUrl { get; set; }
        public string Keyword { get; set; }
        //public string QrcodeUrl { get; set; 
        public int OrderNumber { get; set; }
        public string BrandName { get; set; }
        public List<int> Tags { get; set; }
        public ProductSdkModel Product { get; set; }
        public int ClickCount { get; set; }
        public string PropsName { get; set; } 
        public int LikeClickCount { get; set; }
        public int TotalClickCouont { get; set; }
        public string CategoryIds { get; set; }
        public string RfidCode { get; set; }
        public string ExpirePeriod { get; set; }
        public string ProductCategories { get; set; }
        /// <summary>
        /// 副标题
        /// </summary>
        public string SubTitle { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// 区域
        /// </summary>
        public string Region { get; set; }
    }

    public class ShowSkuInfo
    {
        public long Id { get; set; }
        public ProductType Type { get; set; }
        public string Name { get; set; }
        public string ProductName { get; set; }
        public long Quantity { get; set; }
        public double Price { get; set; }
        public double PromPrice { get; set; }
        public string ImageUrl { get; set; }
        public string TagIconUrl { get; set; }
        public string Keyword { get; set; }
        //public string QrcodeUrl { get; set; 
        public int OrderNumber { get; set; }
        public string BrandName { get; set; }
        public List<int> Tags { get; set; }
        public int ClickCount { get; set; }
        public string PropsName { get; set; }
        public int LikeClickCount { get; set; }
        public int TotalClickCouont { get; set; }
        public string CategoryIds { get; set; }
        public string RfidCode { get; set; }
        public string SkuId { get;  set; }
    }
}
