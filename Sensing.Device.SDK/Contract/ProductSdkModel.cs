using System;
using System.Collections.Generic;
using System.Text;

namespace Sensing.SDK.Contract
{
    public class ProductSdkModel
    {
        public long Id { get; set; }
        public string ItemId { get; set; }
        public long Num { get; set; }
        public string Title { get; set; }
        public virtual long? OrganizationUnitId { get; set; }

        public int OrderNumber { get; set; }

        public string GroupQrCodeInfo { get; set; }
        /// <summary>
        /// 一句话营销.
        /// </summary>
        public string SubTitle { get; set; }
        /// <summary>
        /// 在当今社会,任何事物都是可以明码标价的,难道不是!
        /// </summary>
        public double Price { get; set; }

        public double PromPrice { get; set; }

        public string Barcode { get; set; }
        /// <summary>
        /// 总销量
        /// </summary>
        public int SalesVolume { get; set; }
        /// <summary>
        /// 万物总有属于他自己的关键字,让别人好找到它.
        /// </summary>
        public string Keywords { get; set; }
        public string PicUrl { get; set; }

        /// <summary>
        /// Thing被专门喜欢的此处.
        /// </summary>
        public int LikeCount { get; set; }

        public string Description { get; set; }

        public bool IsFromBrand { get; set; }

        public string SellerId { get; set; }

        public string OuterId { get; set; }

        public string FromType { get; set; }

        public bool HasRealSkus { get; set; }

        public IEnumerable<ProductDecideImageViewModel> PropImgs { get; set; }

        public int[] CategoryIds { get; set; }

        ///// <summary>
        ///// 商品属性值的Ids.
        ///// </summary>
        public long[] PropValueIds { get; set; }

        public long[] TagIds { get; set; }
        public IEnumerable<SkuSdkModel> Skus { get; set; }

        public IEnumerable<EntityFileSdkModel> ItemImagesOrVideos { get; set; }

        public IEnumerable<OnlineStoreInfoViewModel> OnlineStoreInfos { get; set; }

        /// <summary>
        /// Sku 针对年龄段
        /// 例：该Sku适合20-25岁，且适合60-70岁的场合，填入【20-25,60-70】多年龄段半角逗号隔开，为空代表无针对。
        /// </summary>
        public string AgeScope { get; set; }

        /// <summary>
        /// Sku 针对性别
        /// 例：男=Male，女=Female 。为空代表无针对。
        /// </summary>
        public string Gender { get; set; }

        /// <summary>
        /// RFID编号
        /// </summary>
        public string RfidCode { get; set; }

        public double? Price2 { get; set; }
        public string Desc { get; set; }
        public long? BrandId { get; set; }
        public int Quantity { get; set; }
        public string AuditStatus { get; set; }
        /// <summary>
        /// 保质期
        /// </summary>
        public string ExpirePeriod { get; set; }

        public IEnumerable<ProductCategories> ProductCategories { get; set; }
        /// <summary>
        /// 区域
        /// </summary>
        public string Region { get; set; }
    }

    public class ProductCategories
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class ProductDecideImageViewModel
    {
        public string PropertyName { get; set; }
        public string ImageUrl { get; set; }
    }

    public class SkuSdkModel
    {
        public long Id { get; set; }

        public string SkuId { get; set; }

        public long Quantity { get; set; }

        public string Barcode { get; set; }

        public string PropsName { get; set; }

        /// <summary>
        /// SKU属性值的Ids.
        /// </summary>
        public long[] PropValueIds { get; set; }

        /// <summary>
        /// 事物都该有个名字来表示
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 在当今社会,任何事物都是可以明码标价的,难道不是!
        /// </summary>
        public double Price { get; set; }

        /// <summary>
        /// 促销价格
        /// </summary>
        public double PromPrice { get; set; }


        /// <summary>
        /// 总销量
        /// </summary>
        public int SalesVolume { get; set; }

        public int LikeCount { get; set; }
        /// <summary>
        /// 万物总有属于他自己的关键字,让别人好找到它.
        /// </summary>
        public string Keywords { get; set; }

        public string PicUrl { get; set; }

        public string Description { get; set; }

        public string OuterId { get; set; }

        public int OrderNumber { get; set; }

        public long[] TagIds { get; set; }

        public string FromType { get; set; }

        public string ColorName { get; set; }

        public IEnumerable<OnlineStoreInfoViewModel> OnlineStoreInfos { get; set; }

        /// <summary>
        /// Sku 针对年龄段
        /// 例：该Sku适合20-25岁，且适合60-70岁的场合，填入【20-25,60-70】多年龄段半角逗号隔开，为空代表无针对。
        /// </summary>
        public string AgeScope { get; set; }

        /// <summary>
        /// Sku 针对性别
        /// 例：男=Male，女=Female 。为空代表无针对。
        /// </summary>
        public string Gender { get; set; }

        /// <summary>
        /// RFID编号
        /// </summary>
        public string RfidCode { get; set; }

        public double? Price2 { get; set; }

        public IEnumerable<EntityFileSdkModel> ItemImagesOrVideos { get; set; }
        public string AuditStatus { get; set; }

    }

    public class EntityFileSdkModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string FileUrl { get; set; }
        public string FromType { get; set; }
        public string Type { get; set; }
        public string Content { get; set; }
        public string Usage { get; set; }
        public int OrderNumber { get; set; }

    }

    public class OnlineStoreInfoViewModel
    {
        /// <summary>
        /// JD,Taobao
        /// </summary>
        public string OnlineStoreName { get; set; }

        /// <summary>
        /// 线上商城的二维码.
        /// </summary>
        public string Qrcode { get; set; }
    }
}
