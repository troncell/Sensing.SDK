using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Sensing.Device.SDK.Dto
{
    public class StoreDto
    {
    
        public int TotalCount { get; set; }
        public List<StoreItem> Items { get; set; }
    
    }

    public class StoreItem
    {
        public string SubKey { get; set; }
        public long StoreId { get; set; }
        public string DisplayName { get; set; }
        public string OuterId { get; set; }
        public string QrCodeExtraInfo { get; set; }
        public string WebAddressUrl { get; set; }
        public string QrCodeUrl { get; set; }
        public string Contact { get; set; }
        public int? CategoryId { get; set; }
        public string AddressDetail { get; set; }
        public DateTime? OpeningTime { get; set; }
        public DateTime? ClosedTime { get; set; }
        public string QrCodeRules { get; set; }
        public string DefaultOnlineShopName { get; set; }
        public string Type { get; set; }
        public int MemberCount { get; set; }
        public string OrganziationUnitName { get; set; }
        public string StoreType { get; set; }
        public string StoreStatus { get; set; }
        public string StoreDevicesInfo { get; set; }
        public int? BrandId { get; set; }
        public string BrandName { get; set; }
        public double? Distance { get; set; }
        public StoreBusinessStatus? StoreBusinessStatus { get; set; }
        public int? LimitPersonNum { get; set; }
        public string ImgUrl { get; set; }
        public string Resolution_Width { get; set; }
        public string Resolution_Height { get; set; }
    }

    public enum StoreBusinessStatus
    {
        [Display(Name = "无限制营业")]
        NoLimit = 0,
        [Display(Name = "正常营业")]
        Normal = 1,
        [Display(Name = "店铺维护，暂停营业")]
        Maintain = 2
        
    }
}