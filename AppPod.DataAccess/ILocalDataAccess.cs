using AppPod.DataAccess.Models;
using Sensing.SDK.Contract;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AppPod.DataAccess
{
    public interface ILocalSensingDataAccess
    {
        ShowProductInfo GetShowProductInfoById(ProductType type, long id);
        string GetQrcode(SkuSdkModel pModel, string staffId);
        string GetQrcode(ProductSdkModel pModel, string staffId);
        string GetQrcode(ShowProductInfo showProductInfo, string staffId);
        List<PropertyInfo> FindReminderAvailablePropertiesInSkus(ShowProductInfo productSdkModel, params string[] nameValues);
        PropertyInfo GetKeyPropertyInfoInSkus(ProductSdkModel product);
        SkuSdkModel GetSkuSdkModelByShowProduct(ShowProductInfo showProductInfo);
        ProductSdkModel FindByShowProduct(ShowProductInfo showProductInfo);
        List<PropertyInfo> GetPropertyInfosInSkus(ProductSdkModel product);
        PropertyInfo GetKeyPropertyInfoInSkus(ShowProductInfo info);
        List<PropertyInfo> GetPropertyInfosInSkus(ShowProductInfo product);
        (string Key, string Value) GetPropertyName(string properties);
        ProductSdkModel FindByScanId(string scanId);
        ProductSdkModel FindByRfidCode(string rfid);
        List<ShowProductInfo> FindSimilar(ShowProductInfo itemId, bool useSameSpu = true, bool useSameCategories = false);
        ProductSdkModel FindByProductId(long productId);
        ProductSdkModel FindBySkuId(long skuId);
        SkuSdkModel FindSkuById(long skuId);
        List<ShowProductInfo> QueryShowProducts(bool onlySpu);
        List<ProductSdkModel> GetProductsByCategroyName(string categroyName);
        List<ProductSdkModel> GetProductsByCategroyNames(string[] categroyNames);
        List<ShowProductInfo> GetShowProductsByCategroyName(string categroyName);
        List<ShowProductInfo> GetShowProductByCategoryNames(int[] categroyIds);
        string GetOnlineStoreStaffId(long staffId);
        string GetOnlineStoreStaffIdByRFID(string rfidCode);
        string GetStoreType();
        string GetLocalImagePath(string path, string category);

        List<TagSdkModel> GetTagInfos(SvcType type);

        List<TagSdkModel> GetTagInfos();

        List<AdsSdkModel> Ads { get; set; }
        List<StaffSdkModel> Staffs { get; set; }
        List<ProductSdkModel> Products { get; set; }
        List<CouponViewModel> Coupons { get; set; }
        List<ProductCategorySDKModel> PCategories { get; set; }


        List<MatchInfoViewModel> Matches { get; set; }
        List<LikeInfoViewModel> Likes { get; set; }
        List<PropertyViewModel> Properties { get; set; }
        List<BrandDto> Brands { get; set; }

        //List<TagSdkModel> Tags { get; set; }


        List<TagSdkModel> AdTags { get; set; }

        List<TagSdkModel> ProductTags { get; set; }

        List<TagSdkModel> DeviceTags { get; set; }

        List<AppInfo> Apps { get; set; }
        List<ActivityGameDto> ActivityGames { get; set; }
        List<AdAndAppTimelineScheduleViewModel> AdAndAppTimelineSchedules { get; set; }

        List<DeviceSoftwareSdkModel> DeviceSoftwares { get; set; }
        DeviceAppPodVersionModel AppPodVersion { get; set; }

        Dictionary<long, IEnumerable<ShowProductInfo>> MatchedProducts { get; set; }

        List<ShowProductInfo> QueryShowProductsByProperties(IDictionary<string, string> keyValues);
        List<ShowProductInfo> SearchShowProductsByName(string searchTerm);
        //ProductInfo FindProductByAttribute(string productAttribute);
        List<ShowProductInfo> SearchProducts(List<Range<float>> priceRanges, List<string> colors, List<int> categories, List<int> tags, List<string> keywords, bool onlySpu = false);

        List<ProductCategorySDKModel> GetCategroyInfos(bool isSpecial = true);
        List<ProductCategorySDKModel> GetUsefulCategroyInfos();
        //List<ProductInfo> GetProductInfos();
        List<CouponViewModel> GetCoupons();
        List<AdsSdkModel> GetAdsInfos();
        //List<StaffInfo> GetStaffInfos();
        void Like(ProductSdkModel productInfo);
        //void Click(ProductInfo productInfo);
        //void LoadProducts();
        //string GetLocalImage(ProductInfo p);
        //string GetLocalCategroyImage(CategroyInfo ca);
        //string GetLocalCategoryIcon(CategroyInfo ca);
        //string GetProgress();
        bool IsCompleted();
        bool Intialize();
        List<AdsSdkModel> ReadAds();
        List<MatchInfoViewModel> ReadProductMatches();
        List<LikeInfoViewModel> ReadProductLikes();
        List<CouponViewModel> ReadCoupons();
        List<ProductSdkModel> ReadProducts();
        List<ProductCategorySDKModel> ReadCategorys();
        List<TagSdkModel> ReadAdTags();
        List<TagSdkModel> ReadProductTags();
        List<TagSdkModel> ReadDeviceTags();
        List<StaffSdkModel> ReadStaffs();
        /// <summary>
        /// 获取一级分类
        /// </summary>
        /// <returns></returns>
        IEnumerable<ProductCategorySDKModel> GetRootCategories();

        /// <summary>
        /// 根据父分类id获取子分类
        /// </summary>
        /// <param name="parentCategoryId"></param>
        /// <returns></returns>
        IEnumerable<ProductCategorySDKModel> GetChildrenCategory(int parentCategoryId);

        Task<List<ProductCommentModel>> GetProductComments(long productId);

        IEnumerable<ShowProductInfo> GetBrandProducts(long brandId);
        IEnumerable<ProductCategorySDKModel> GetBrandCategories(long brandId);

        IEnumerable<ShowProductInfo> QueryProducts(long brandId, int categoryId);

        #region Metas
        List<MetaPhysicsDto> Metas { get; set; }
        List<DateMetaphysicsDto> DateMetas { get; set; }

        IEnumerable<MetaPhysicsDto> GetAllAstro();
        DateMetaphysicsDto GetNowOrLatestLuckyByMetaId(long metaId);
        #endregion

        IEnumerable<BrandDto> GetBrandsByMainCategory(int categoryId);
        IEnumerable<AdsSdkModel> FindAdsByTagName(string tagName);
        void FindAllMatch();
        void RemoveFrontBrandName();
        void RemovePrefixText(string text);

        ShowProductInfo GetShowProductByOutId(string outId);
        ShowProductInfo GetShowProductBySku(long skuId);
        void ReadClickCounts(List<ClickInfo> clickInfoData);
        void ReadLikeClickCounts(List<ClickInfo> clickInfoData);
        void ReadAllClickCounts(List<ClickInfo> clickInfoData);
        void SetKeyword(string keyword);
        IEnumerable<ShowProductInfo> QueryByKeyword(string keyword);
        string GetStoreId();

    }
}
