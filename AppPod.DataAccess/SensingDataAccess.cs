using AppPod.DataAccess.Models;
using Newtonsoft.Json;
using Sensing.SDK.Contract;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using TinyPinyin;

namespace AppPod.DataAccess
{
    public class Range<T>
    {
        public T Min { get; set; }
        public T Max { get; set; }
    }

    public enum SvcType
    {
        Product,
        DeviceCenter,
        Ads
    }
    public class SensingDataAccess : ILocalSensingDataAccess
    {
        public static string AppPodDataDirectory { get; set; }
        public static DeviceSetting DeviceSetting { get; set; }

        private List<ShowProductInfo> mShowProducts;
        public SensingDataAccess(bool isAutoFindAppPodDataDirecotry = true)
        {
            if (isAutoFindAppPodDataDirecotry)
            {
                AppPodDataDirectory = FindAppPodDataFolder();
                DeviceSetting = FindDeviceSetting();
            }

            Initialize();
        }

        public SensingDataAccess(string appPodDataDirectory)
        {
            AppPodDataDirectory = appPodDataDirectory;
        }

        /// <summary>
        /// 获取一级分类
        /// </summary>
        /// <returns></returns>
        public IEnumerable<ProductCategorySDKModel> GetRootCategories()
        {
            if (PCategories == null)
                return new List<ProductCategorySDKModel>();
            var roots = PCategories.Where(p => p.ParentCategoryId == 0 || p.ParentCategoryId == p.Id);
            return roots.ToList();
        }


        public AdAndAppTimelineScheduleViewModel GetTodayAdAndAppTimelineContent()
        {
            var todayTimelineContent = AdAndAppTimelineSchedules.FirstOrDefault(t => t.Date.ToString("yyyy-MM-dd") == DateTime.Now.ToString("yyyy-MM-dd"));
            return todayTimelineContent;
        }

        public string GetStoreId()
        {
            try{
                string appPodFolder = AppPodDataDirectory;
                if (appPodFolder != null)
                {
                    string deviceJson = File.ReadAllText(Path.Combine(appPodFolder, "Device.json"));
                    var deviceInfo = JsonConvert.DeserializeObject<DeviceOutput>(deviceJson);
                    if (deviceInfo != null)
                    {
                        return deviceInfo.StoreOuterId;
                    }
                }
            }
           catch (Exception)
            {
            }
            return null;
        }


        /// <summary>
        /// 根据父分类id获取子分类
        /// </summary>
        /// <param name="parentCategoryId"></param>
        /// <returns></returns>
        public IEnumerable<ProductCategorySDKModel> GetChildrenCategory(int parentCategoryId)
        {
            IEnumerable<ProductCategorySDKModel> children;
            children = PCategories.Where(p => p.ParentCategoryId == parentCategoryId && p.ParentCategoryId != p.Id);
            return children.ToList();
        }
        



        public string GetQrcode(ShowProductInfo showProductInfo, string staffId)
        {
            if (showProductInfo == null) return null;
            string qrcode = string.Empty;
            var storeType = GetStoreType();
            if (showProductInfo.Type == ProductType.Product)
            {
                var pModel = FindByShowProduct(showProductInfo);
                qrcode = pModel.OnlineStoreInfos.FirstOrDefault(s => s.OnlineStoreName == storeType)?.Qrcode;
                if(qrcode == null)
                {
                    qrcode = pModel.OnlineStoreInfos.FirstOrDefault()?.Qrcode;
                }
            }
            else
            {
                var sModel = FindSkuById(showProductInfo.Id);
                qrcode = sModel.OnlineStoreInfos.FirstOrDefault(s => s.OnlineStoreName == storeType)?.Qrcode;
                if (qrcode == null)
                {
                    qrcode = sModel.OnlineStoreInfos.FirstOrDefault()?.Qrcode;
                }
            }
            if (string.IsNullOrEmpty(qrcode)) return null;
            if (!string.IsNullOrEmpty(staffId))
            {
                if (qrcode.EndsWith("&"))
                {
                    qrcode = $"{qrcode}saleId={staffId}";
                }
                else
                {
                    qrcode = $"{qrcode}&saleId={staffId}";
                }
            }
            return qrcode;
        }

        public string GetQrcode(ProductSdkModel pModel, string staffId)
        {
            if (pModel == null) return null;
            string qrcode = string.Empty;
            var storeType = GetStoreType();
            qrcode = pModel.OnlineStoreInfos?.FirstOrDefault(s => s.OnlineStoreName == storeType)?.Qrcode;
            if (qrcode == null)
            {
                qrcode = pModel.OnlineStoreInfos.FirstOrDefault()?.Qrcode;
            }
            if (string.IsNullOrEmpty(qrcode)) return null;
            if (!string.IsNullOrEmpty(staffId))
            {
                if (qrcode.EndsWith("&"))
                {
                    qrcode = $"{qrcode}saleId={staffId}";
                }
                else
                {
                    qrcode = $"{qrcode}&saleId={staffId}";
                }
            }
            return qrcode;
        }

        public string GetQrcode(SkuSdkModel sModel, string staffId)
        {
            if (sModel == null) return null;
            var storeType = GetStoreType();
            string qrcode = sModel.OnlineStoreInfos.FirstOrDefault(s => s.OnlineStoreName == storeType)?.Qrcode;
            if (qrcode == null)
            {
                qrcode = sModel.OnlineStoreInfos.FirstOrDefault()?.Qrcode; 
            }
            if (string.IsNullOrEmpty(qrcode)) return null;
            if (!string.IsNullOrEmpty(staffId))
            {
                if (qrcode.EndsWith("&"))
                {
                    qrcode = $"{qrcode}saleId={staffId}";
                }
                else
                {
                    qrcode = $"{qrcode}&saleId={staffId}";
                }
            }
            return qrcode;
        }

        #region Bussiness Logical Data
        public string GetOnlineStoreStaffId(long staffId)
        {
            var storeType = GetStoreType();
            var staff = Staffs.Find(s => s.Id == staffId);
            if (staff == null) return null;
            var onlineStaff = staff.OnlineStoreInfos.AsQueryable().FirstOrDefault(s => s.OnlineStoreName == storeType);
            if (onlineStaff == null) return null;
            return onlineStaff.Code;
        }

        public string GetOnlineStoreStaffIdByRFID(string rfidCode)
        {
            var staff = Staffs.Find(s => s.RfidCode == rfidCode);
            if (staff == null) return null;
            return GetOnlineStoreStaffId(staff.Id);
        }

        public ProductSdkModel FindByProductId(long id)
        {
            return Products?.Find(p => p.Id == id);
        }

        public ProductSdkModel FindBySkuId(long skuId)
        {
            return Products?.FirstOrDefault(p => p.Skus.Any(s => s.Id == skuId));
        }

        public ShowProductInfo GetShowProductInfoById(ProductType type, long id)
        {
            if (type == ProductType.Product)
            {
                var pModel = Products?.Find(p => p.Id == id);
                if (pModel != null)
                {
                    return new ShowProductInfo
                    {
                        Id = id,
                        ImageUrl = GetLocalImagePath(pModel.PicUrl, "Products"),
                        Name = pModel.Title,
                        Price = pModel.Price,
                        //QrcodeUrl = pModel.OnlineStoreInfos.FirstOrDefault(s => s.OnlineStoreType == storeType)?.Qrcode,
                        Type = ProductType.Product,
                        TagIconUrl = FindTagIcon(pModel.TagIds, SvcType.Product),
                        Product = pModel
                    };
                }
            }
            if (type == ProductType.Sku)
            {
                var pModel = FindBySkuId(id);
                var firstSku = FindSkuById(id);
                if (firstSku != null && pModel != null)
                {
                    var keyImage = pModel.PropImgs.FirstOrDefault(p => firstSku.PropsName.Contains(p.PropertyName));
                    return new ShowProductInfo
                    {
                        Id = firstSku.Id,
                        ImageUrl = GetLocalImagePath(keyImage.ImageUrl??firstSku.PicUrl, "Products"),
                        Quantity = firstSku.Quantity,
                        Name = firstSku.Title,
                        Price = firstSku.Price,
                        //QrcodeUrl = prod.OnlineStoreInfos.FirstOrDefault(s => s.OnlineStoreType == storeType)?.Qrcode,
                        TagIconUrl = FindTagIcon(firstSku.TagIds, SvcType.Product),
                        Type = ProductType.Sku,
                        Product = pModel
                    };
                }
            }
            return null;
        }

        public ProductSdkModel FindByShowProduct(ShowProductInfo showProductInfo)
        {
            if (showProductInfo == null) return null;
            if (showProductInfo.Type == ProductType.Product) return FindByProductId(showProductInfo.Id);
            return FindBySkuId(showProductInfo.Id);
        }

        public SkuSdkModel FindSkuById(long skuId)
        {
            return Products?.FirstOrDefault(p => p.Skus != null && p.Skus.Any(s => s.Id == skuId))?.Skus.FirstOrDefault(s => s.Id == skuId);
        }

        public ProductSdkModel FindByScanId(string skc)
        {
            return Products?.FirstOrDefault(p => p.Skus.Any(s => s.SkuId.Contains(skc)));
        }

        public ProductSdkModel FindByRfidCode(string rfid)
        {
            return Products?.FirstOrDefault(p => p.Skus.Any(s => (s.RfidCode?.StartsWith(rfid)??false) || (s.OuterId?.StartsWith(rfid)??false) || (s.SkuId?.StartsWith(rfid)??false)));
        }

        public  string GetLocalImagePath(string path, string category)
        {
            if (string.IsNullOrEmpty(path)) return null;
            var localPath = Extensions.ExtractSchema(path);
            return $"{SensingDataAccess.AppPodDataDirectory}\\{category}\\res\\{localPath}";
        }

        public static string GetAdsLocalFile(string url)
        {
            if (string.IsNullOrEmpty(url))
                return null;
            var localPath = Extensions.ExtractSchema(url);
            return $"{SensingDataAccess.AppPodDataDirectory}\\ads\\res\\{localPath}";
        }

        private string FindTagIcon(long[] tagIds, SvcType type )
        {
            List<TagSdkModel> tags = null;
            switch (type)
            {
                case SvcType.Product:
                    tags = ProductTags;
                    break;
                case SvcType.DeviceCenter:
                    tags = DeviceTags;
                    break;
                case SvcType.Ads:
                    tags = AdTags;
                    break;
                default:
                    break;
            }

            if (tags == null || tags.Count == 0) return null;
            var tag = tags.Find(t => tagIds.Any(id => id == t.Id) && t.IsSpecial == true);
            if (tag != null) return tag.GetLocalIconFile();
            return null;
        }

        public List<TagSdkModel> GetTagInfos(SvcType type)
        {
            List<TagSdkModel> tags = null;
            switch (type)
            {
                case SvcType.Product:
                    tags = ProductTags;
                    break;
                case SvcType.DeviceCenter:
                    tags = DeviceTags;
                    break;
                case SvcType.Ads:
                    tags = AdTags;
                    break;
                default:
                    break;
            }

            if (tags == null || tags.Count == 0) return null;
            var result = tags.Where(t => t.IsSpecial == true).ToList();
            return result;
        }

        public List<TagSdkModel> GetTagInfos()
        {
            List<TagSdkModel> tags = new List<TagSdkModel>();
            if(AdTags != null) tags.AddRange(AdTags);
            if (ProductTags != null) tags.AddRange(ProductTags);
            if (DeviceTags != null)  tags.AddRange(DeviceTags);

            if (tags == null || tags.Count == 0) return null;
            var result = tags.Where(t => t.IsSpecial == true).ToList();
            return result;
        }

        public List<ShowProductInfo> QueryShowProducts(bool onlySpu = false)
        {
            if (Products == null || Products.Count == 0) return null;
            if (mShowProducts != null) return mShowProducts;
            string storeType = GetStoreType();
            if (onlySpu)
            {
                var infos = Products.Select(pModel => new ShowProductInfo
                {
                    Id = pModel.Id,
                    ImageUrl = GetLocalImagePath(pModel.PicUrl, "Products"),
                    Name = pModel.Title,
                    Price = pModel.Price,
                    PromPrice = pModel.PromPrice,
                    ProductName = pModel.Title,
                    Quantity = Math.Max(pModel.Num, pModel.Skus?.Sum(s => s.Quantity) ?? 0),
                    //QrcodeUrl = pModel.OnlineStoreInfos.FirstOrDefault(s => s.OnlineStoreType == storeType)?.Qrcode,
                    Type = ProductType.Product,
                    TagIconUrl = FindTagIcon(pModel.TagIds, SvcType.Product),
                    Product = pModel
                }).ToList();
                mShowProducts = infos;
            }
            else
            {
                var showProducts = new List<ShowProductInfo>();
                foreach (var prod in Products)
                {
                    if (prod.Skus == null)
                    {
                        if (prod.HasRealSkus == false)
                        {
                            showProducts.Add(new ShowProductInfo
                            {
                                Id = prod.Id,
                                ImageUrl = GetLocalImagePath(prod.PicUrl, "Products"),
                                Name = prod.Title,
                                Price = prod.Price,
                                PromPrice = prod.PromPrice,
                                ProductName = prod.Title,
                                Quantity =Math.Max(prod.Num, prod.Skus?.Sum(s => s.Quantity)??0), 
                                Type = ProductType.Product,
                                TagIconUrl = FindTagIcon(prod.TagIds, SvcType.Product),
                                Product = prod,
                            });
                        }
                        continue;
                    }
                    if (prod.Skus != null && prod.Skus.Count() == 0)
                    {
                        if (!prod.HasRealSkus)
                        {
                            showProducts.Add(new ShowProductInfo
                            {
                                Id = prod.Id,
                                ImageUrl = GetLocalImagePath(prod.PicUrl, "Products"),
                                Quantity = prod.Num,
                                Name = prod.Title,
                                ProductName = prod.Title,
                                Price = prod.Price,
                                PromPrice = prod.PromPrice,
                                
                                //QrcodeUrl = prod.OnlineStoreInfos.FirstOrDefault(s => s.OnlineStoreType == storeType)?.Qrcode,
                                Type = ProductType.Product,
                                TagIconUrl = FindTagIcon(prod.TagIds, SvcType.Product),
                                Product = prod
                            });
                        }
                        continue;
                    }
                    if (prod.PropImgs != null && prod.PropImgs.Count() > 0)
                    {
                        foreach (var pImg in prod.PropImgs)
                        {
                            var keyProps = pImg.PropertyName;
                            var firstSku = prod.Skus.AsQueryable().FirstOrDefault(s => s.PropsName != null && s.PropsName.Contains(keyProps));
                            if (firstSku != null)
                            {
                                showProducts.Add(new ShowProductInfo
                                {
                                    Id = firstSku.Id,
                                    ImageUrl = GetLocalImagePath(pImg.ImageUrl??firstSku.PicUrl, "Products"),
                                    Quantity = firstSku.Quantity,
                                    Name = prod.Title,
                                    ProductName = prod.Title,
                                    Price = firstSku.Price,
                                    PromPrice = firstSku.PromPrice,
                                    //QrcodeUrl = prod.OnlineStoreInfos.FirstOrDefault(s => s.OnlineStoreType == storeType)?.Qrcode,
                                    TagIconUrl = FindTagIcon(firstSku.TagIds, SvcType.Product),
                                    Type = ProductType.Sku,
                                    Product = prod,
                                    PropsName = firstSku.PropsName

                                });
                            }
                        }
                    }
                    else
                    {
                        var firstSku = prod.Skus.First();
                        if (firstSku != null)
                        {
                            showProducts.Add(new ShowProductInfo
                            {
                                Id = firstSku.Id,
                                ImageUrl = GetLocalImagePath(prod.PicUrl, "Products"),
                                Quantity = firstSku.Quantity,
                                Name = firstSku.Title,
                                ProductName = prod.Title,
                                Price = firstSku.Price,
                                PromPrice = firstSku.PromPrice,
                                //QrcodeUrl = prod.OnlineStoreInfos.FirstOrDefault(s => s.OnlineStoreType == storeType)?.Qrcode,
                                TagIconUrl = FindTagIcon(firstSku.TagIds, SvcType.Product),
                                Type = ProductType.Sku,
                                Product = prod,
                                PropsName = firstSku.PropsName
                            });
                        }
                    }
                }
                mShowProducts = showProducts;
            }
            mShowProducts.ForEach(p => {
                p.BrandName = Brands.FirstOrDefault(b => b.Id == p.Product.BrandId)?.Name;
            });
            return mShowProducts;
        }



        public List<ProductSdkModel> GetProductsByCategroyName(string categroyName)
        {
            var category = FindCategoryByName(categroyName);
            if (category != null)
            {
                return Products.Where(p => p.CategoryIds != null && p.CategoryIds.Contains(category.Id)).ToList();
            }
            return null;
        }

        public List<ShowProductInfo> GetShowProductsByCategroyName(string categroyName)
        {
            var category = FindCategoryByName(categroyName);
            if (category != null)
            {
                return mShowProducts.Where(p => p.Product.CategoryIds != null && p.Product.CategoryIds.Contains(category.Id)).ToList();
            }
            return null;
        }

        public List<ProductSdkModel> GetProductsByCategroyNames(string[] categroyNames)
        {
            var ids = FindCategoryIdsByNames(categroyNames);
            if (Products == null || ids == null) return null;
            var prods = new List<ProductSdkModel>();
            foreach (var p in Products)
            {
                foreach (var id in ids)
                {
                    if (p.CategoryIds.Contains(id))
                    {
                        prods.Add(p);
                        break;
                    }
                }
            }
            return prods;
        }

        public List<ShowProductInfo> GetShowProductByCategoryNames(int[] categroyIds)
        {
            return mShowProducts.Where(p => p.Product.CategoryIds.Intersect(categroyIds).Count() > 0).ToList();
        }

        public ProductCategorySDKModel FindCategoryByName(string categoryName)
        {
            return PCategories.Find(p => p.Name.Contains(categoryName));
        }

        public List<int> FindCategoryIdsByNames(string[] categoryNames)
        {
            return PCategories.Where(p => categoryNames.Any(c => c == p.Name)).Select(s => s.Id).ToList();
        }
        public List<ProductCategorySDKModel> GetCategroyInfos(bool isSpecial = true)
        {
            return PCategories?.Where(p => p.IsSpecial == isSpecial).ToList();
        }

        public List<ProductCategorySDKModel> GetUsefulCategroyInfos()
        {
            if (PCategories == null || PCategories.Count == 0) return null;
            if (Products == null || Products.Count == 0) return null;
            List<ProductCategorySDKModel> results = new List<ProductCategorySDKModel>();
            foreach (var pCategory in PCategories)
            {
                if (Products.Any(p => p.CategoryIds.Contains(pCategory.Id)))
                {
                    results.Add(pCategory);
                }
            }
            return results;
        }

        public List<CouponViewModel> GetCoupons()
        {
            if (Coupons == null)
                return new List<CouponViewModel>();
            return Coupons.ToList();
        }


        public bool CanAddFilter(ProductSdkModel product, List<Range<float>> priceRanges, List<string> colors, List<int> tags, List<int> categories, List<string> keywords)
        {
            var priceOk = false;
            if (priceRanges != null && priceRanges.Count > 0)
            {
                foreach (var range in priceRanges)
                {
                    if (product.Price >= range.Min && product.Price <= range.Max)
                    {
                        priceOk = true;
                        break;
                    }
                }
            }
            else
            {
                priceOk = true;
            }

            var colorOK = false;
            if (colors != null && colors.Count > 0)
            {
                foreach (var color in colors)
                {
                    if ((!string.IsNullOrEmpty(product.Title) && product.Title.Contains(color))
                        || (!string.IsNullOrEmpty(product.Description) && product.Description.Contains(color)) )
                    {
                        colorOK = true;
                        break;
                    }
                }
            }
            else
            {
                colorOK = true;
            }

            var tagOK = false;
            if (tags != null && tags.Count > 0)
            {
                if (product.TagIds != null)
                {
                    foreach (var tag in tags)
                    {
                        if (product.TagIds.Contains(tag))
                        {
                            tagOK = true;
                            break;
                        }
                    }
                }
            }
            else
            {
                tagOK = true;
            }

            var categoryOK = false;
            if (categories != null && categories.Count > 0)
            {
                if (product.CategoryIds != null)
                {
                    var categoryIds  = PCategories.Where(c => categories.Contains(c.Id)).SelectMany(c => c.Ids);
                    categoryOK = product.CategoryIds.Intersect(categoryIds).Count() > 0;
                }
            }
            else
            {
                categoryOK = true;
            }

            var keywordOK = false;
            if (keywords != null && keywords.Count > 0)
            {
                if (product.Keywords != null)
                {
                    foreach (var keyword in keywords)
                    {
                        if (product.Keywords.Contains(keyword))
                        {
                            keywordOK = true;
                            break;
                        }
                    }
                }
            }
            else
            {
                keywordOK = true;
            }

            return priceOk && colorOK && tagOK && categoryOK && keywordOK;
        }

        public bool CanAddFilter(SkuSdkModel sku, List<Range<float>> priceRanges, List<string> colors, List<int> tags, List<string> keywords)
        {
            var priceOk = false;
            if (priceRanges != null && priceRanges.Count > 0)
            {
                foreach (var range in priceRanges)
                {
                    if (sku.Price >= range.Min && sku.Price <= range.Max)
                    {
                        priceOk = true;
                        break;
                    }
                }
            }
            else
            {
                priceOk = true;
            }

            var colorOK = false;
            if (colors != null && colors.Count > 0)
            {
                foreach (var color in colors)
                {
                    if ((!string.IsNullOrEmpty(sku.Title) && sku.Title.Contains(color))
                        || (!string.IsNullOrEmpty(sku.Description) && sku.Description.Contains(color))
                        || (!string.IsNullOrEmpty(sku.ColorName) && sku.ColorName.Contains(color))
                        || (!string.IsNullOrEmpty(sku.PropsName) && sku.PropsName.Contains(color))
                        )
                    {
                        colorOK = true;
                        break;
                    }
                }
            }
            else
            {
                colorOK = true;
            }

            var tagOK = false;
            if (tags != null && tags.Count > 0)
            {
                if (sku.TagIds != null)
                {
                    foreach (var tag in tags)
                    {
                        if (sku.TagIds.Contains(tag))
                        {
                            tagOK = true;
                            break;
                        }
                    }
                }
            }
            else
            {
                tagOK = true;
            }

            var keywordOK = false;
            if (keywords != null && keywords.Count > 0)
            {
                if (sku.Keywords != null)
                {
                    foreach (var keyword in keywords)
                    {
                        if (sku.Keywords.Contains(keyword))
                        {
                            keywordOK = true;
                            break;
                        }
                    }
                }
            }
            else
            {
                keywordOK = true;
            }

            return priceOk && colorOK && tagOK && keywordOK;
        }

        private bool ProductIsOK(ProductSdkModel product, List<int> categories)
        {
            if (categories == null) return true;
            if (categories.Count < 1)
                return true;
            foreach (var id in categories)
            {
                var category = PCategories.FirstOrDefault(p => p.Id == id);
                if (category == null)
                    continue;
                if (category.Ids.Intersect(product.CategoryIds).Count() > 0)
                    return true;
            }
            return false;
        }

        public List<ShowProductInfo> SearchProducts(List<Range<float>> priceRanges, List<string> colors, List<int> categories, List<int> tags, List<string> keywords, bool onlySpu = false)
        {
            if (Products == null || Products.Count == 0) return null;
            string storeType = GetStoreType();
            if (onlySpu)
            {
                var infos = Products.Where(p => CanAddFilter(p, priceRanges, colors, tags, categories, keywords)).Select(pModel => new ShowProductInfo
                {
                    Id = pModel.Id,
                    ImageUrl = GetLocalImagePath(pModel.PicUrl, "Products"),
                    Name = pModel.Title,
                    Price = pModel.Price,
                    //QrcodeUrl = pModel.OnlineStoreInfos.FirstOrDefault(s => s.OnlineStoreType == storeType)?.Qrcode,
                    Type = ProductType.Product,
                    TagIconUrl = FindTagIcon(pModel.TagIds, SvcType.Product),
                    Product = pModel,
                    Keyword = pModel.Keywords
                }).ToList();
                return infos;
            }
            else
            {
                var showProducts = new List<ShowProductInfo>();
                foreach (var prod in Products)
                {
                    if (prod.Skus == null)
                    {
                        if (prod.HasRealSkus == false)
                        {
                            if (CanAddFilter(prod, priceRanges, colors, tags, categories, keywords))
                            {
                                showProducts.Add(new ShowProductInfo
                                {
                                    Id = prod.Id,
                                    ImageUrl = GetLocalImagePath(prod.PicUrl, "Products"),
                                    Name = prod.Title,
                                    Price = prod.Price,
                                    Quantity = prod.Num,
                                    Type = ProductType.Product,
                                    TagIconUrl = FindTagIcon(prod.TagIds, SvcType.Product),
                                    Product = prod,
                                    Keyword = prod.Keywords
                                });
                            }
                        }
                        continue;
                    }
                    if (prod.Skus != null && prod.Skus.Count() == 0)
                    {
                        if (!prod.HasRealSkus)
                        {
                            if (CanAddFilter(prod, priceRanges, colors, tags, categories, keywords))
                            {
                                showProducts.Add(new ShowProductInfo
                                {
                                    Id = prod.Id,
                                    ImageUrl = GetLocalImagePath(prod.PicUrl, "Products"),
                                    Quantity = prod.Num,
                                    Name = prod.Title,
                                    Price = prod.Price,
                                    //QrcodeUrl = prod.OnlineStoreInfos.FirstOrDefault(s => s.OnlineStoreType == storeType)?.Qrcode,
                                    Type = ProductType.Product,
                                    TagIconUrl = FindTagIcon(prod.TagIds, SvcType.Product),
                                    Product = prod,
                                    Keyword = prod.Keywords
                                });
                            }
                        }
                        continue;
                    }
                    if (prod.PropImgs != null && prod.PropImgs.Count() > 0)
                    {
                        if (ProductIsOK(prod, categories))
                        {
                            foreach (var pImg in prod.PropImgs)
                            {
                                var keyProps = pImg.PropertyName;
                                var firstSku = prod.Skus.AsQueryable().FirstOrDefault(s => s.PropsName != null && s.PropsName.Contains(keyProps) && CanAddFilter(s, priceRanges, colors, tags, keywords));
                                if (firstSku != null)
                                {
                                    showProducts.Add(new ShowProductInfo
                                    {
                                        Id = firstSku.Id,
                                        ImageUrl = GetLocalImagePath(firstSku.PicUrl, "Products"),
                                        Quantity = firstSku.Quantity,
                                        Name = firstSku.Title,
                                        Price = firstSku.Price,
                                        //QrcodeUrl = prod.OnlineStoreInfos.FirstOrDefault(s => s.OnlineStoreType == storeType)?.Qrcode,
                                        TagIconUrl = FindTagIcon(firstSku.TagIds, SvcType.Product),
                                        Type = ProductType.Sku,
                                        Product = prod,
                                        Keyword = prod.Keywords
                                    });

                                }
                            }
                        }
                    }
                }
                return showProducts;
            }
        }

        //static HanyuPinyinOutputFormat format;
        public void Initialize()
        {
            //format = new HanyuPinyinOutputFormat();
            //format.CaseType = HanyuPinyinCaseType.UPPERCASE;
            //format.ToneType = HanyuPinyinToneType.WITHOUT_TONE;
        }

        public static string FirstChars(string name)
        {
            string returnString = "";
            char[] chars = name.ToCharArray();
            foreach (char c in chars)
            {
                if (PinyinHelper.IsChinese(c))
                {
                    string result = PinyinHelper.GetPinyin(c);
                    if (result.Length > 0)
                    {
                        var first = result[0];
                        //if (first.Length > 0)
                        //{
                            returnString += first;
                        //}
                    }
                }
                else
                {
                    returnString += c;
                }
            }
            return returnString;
        }
        public static bool IsChineseChar(char c)
        {
            if (c >= 0x4e00 && c <= 0x9fbb)
            {
                return true;
            }
            return false;
        }


        public List<ShowProductInfo> SearchShowProductsByName(string searchTerm)
        {
            if (string.IsNullOrEmpty(searchTerm)) return mShowProducts;

            List<ShowProductInfo> resultProductList = new List<ShowProductInfo>();

            foreach (var showInfo in mShowProducts)
            {
                bool isNameContains = showInfo.Name.IndexOf(searchTerm, StringComparison.OrdinalIgnoreCase) >= 0;  // 英文情况，模糊大小写判断是否包含

                if (showInfo.Product.ItemId.Contains(searchTerm) || isNameContains || FirstChars(showInfo.Name).Contains(searchTerm))
                {
                    resultProductList.Add(showInfo);
                }
            }

            return resultProductList;
        }

        //public List<ShowProductInfo> SearchShowProductsById(string searchTerm)
        //{
        //    if (string.IsNullOrEmpty(searchTerm))
        //        return new List<ShowProductInfo>();

        //    return mShowProducts.Where(p => p.Type == ProductType.Sku && p.Id.ToString() == searchTerm).ToList();
        //}

        public void Like(ProductSdkModel productInfo)
        {
            //throw new NotImplementedException();
        }
        public List<AdsSdkModel> GetAdsInfos()
        {
            return Ads;
        }
        public bool IsCompleted()
        {
            return true;
        }
        #endregion

        #region AppPod Base
        public static string FindAppPodRootFolder()
        {
            var exeRoot = AppDomain.CurrentDomain.BaseDirectory;
            DirectoryInfo root = new DirectoryInfo(exeRoot);
            while (root != null)
            {
                if (File.Exists(Path.Combine(root.FullName, "AppPod.exe")))
                {
                    return root.FullName;
                }
                root = root.Parent;
            }
            return null;
        }

        public static string FindAppPodDataFolder()
        {
            var rootFolder = FindAppPodRootFolder();
            if (rootFolder == null)
                return null;
            return Path.Combine(rootFolder, "AppPodData");

        }

        private static DeviceSetting FindDeviceSetting()
        {
            var rootFolder = FindAppPodRootFolder();
            if (rootFolder == null)
                return null;
            var sqlconnection = new SQLite.SQLiteConnection(rootFolder + "/_data/AppPodData.db");
            return sqlconnection.Table<DeviceSetting>().FirstOrDefault();
        }

        private static string FindResourceFolder()
        {
            var exeRoot = AppDomain.CurrentDomain.BaseDirectory;
            DirectoryInfo root = new DirectoryInfo(exeRoot);
            while (root != null)
            {
                if (File.Exists(Path.Combine(root.FullName, "AppPod.exe")))
                {
                    return root.FullName + "/AppPodData/";
                }
                root = root.Parent;
            }
            return null;
        }
        #endregion

        public List<AdsSdkModel> Ads { get; set; }
        public List<StaffSdkModel> Staffs { get; set; }
        //public List<TagSdkModel> Tags { get; set; }

        public List<TagSdkModel> AdTags { get; set; }
        public List<TagSdkModel> ProductTags { get; set; }
        public List<TagSdkModel> DeviceTags { get; set; }

        public List<ProductSdkModel> Products { get; set; }
        public List<CouponViewModel> Coupons { get; set; }
        public List<ProductCategorySDKModel> PCategories { get; set; }
        public List<MatchInfoViewModel> Matches { get; set; }
        public List<LikeInfoViewModel> Likes { get; set; }
        public List<PropertyViewModel> Properties { get; set; }
        public List<ProductCommentModel> ProductComments { get; set; }
        public List<BrandDto> Brands { get; set; }
        public List<MetaPhysicsDto> Metas { get; set; }
        public List<DateMetaphysicsDto> DateMetas { get; set; }
        public Dictionary<long, IEnumerable<ShowProductInfo>> MatchedProducts { get; set; }
        public List<AppInfo> Apps { get; set; }
        public List<DeviceSoftwareSdkModel> DeviceSoftwares { get; set; }
        
        public List<ActivityGameDto> ActivityGames { get; set; }
        public  List<AdAndAppTimelineScheduleViewModel> AdAndAppTimelineSchedules { get; set; }

        public DeviceAppPodVersionModel AppPodVersion { get; set; }

        #region Read Data from Local Json.
        public List<AdsSdkModel> ReadAds()
        {
            var path = $"{AppPodDataDirectory}/Ads/Ads.json";
            if (!File.Exists(path)) return null;
            string json = File.ReadAllText(path);
            return JsonConvert.DeserializeObject<List<AdsSdkModel>>(json);
        }

        public List<MatchInfoViewModel> ReadProductMatches()
        {
            var path = $"{AppPodDataDirectory}/Products/Matches.json";
            if (!File.Exists(path)) return null;
            string json = File.ReadAllText(path);

            var matches = JsonConvert.DeserializeObject<List<MatchInfoViewModel>>(json);

            foreach (var match in matches)
            {
                foreach (var item in match.MatchItems)
                {
                    var product = FindBySkuId(item.SkuId);

                    if (product != null)
                    {
                        item.SkuPicUrl = GetLocalImagePath(product.PicUrl, "Products");
                    }
                }
            }
            return matches;
        }

        public List<LikeInfoViewModel> ReadProductLikes()
        {
            var path = $"{AppPodDataDirectory}/Products/Likes.json";
            if (!File.Exists(path)) return null;
            string json = File.ReadAllText(path);

            var likes = JsonConvert.DeserializeObject<List<LikeInfoViewModel>>(json);
            foreach (var like in likes)
            {
                foreach (var item in like.LikeItems)
                {
                    var product = FindBySkuId(item.SkuId);

                    if (product != null)
                    {
                        item.SkuPicUrl = GetLocalImagePath(product.PicUrl, "Products");
                    }
                }
            }
            return likes;
        }

        public List<PropertyViewModel> ReadProperties()
        {
            var path = $"{AppPodDataDirectory}/Products/Properties.json";
            if (!File.Exists(path)) return null;
            string json = File.ReadAllText(path);

            var properties = JsonConvert.DeserializeObject<List<PropertyViewModel>>(json);
            return properties;
        }

        public List<CouponViewModel> ReadCoupons()
        {
            var path = $"{AppPodDataDirectory}/Products/Coupons.json";
            if (!File.Exists(path)) return null;
            string json = File.ReadAllText(path);
            return JsonConvert.DeserializeObject<List<CouponViewModel>>(json);
        }

        public List<ProductSdkModel> ReadProducts()
        {
            var path = $"{AppPodDataDirectory}/Products/Products.json";
            if (!File.Exists(path)) return null;
            string json = File.ReadAllText(path);
            return JsonConvert.DeserializeObject<List<ProductSdkModel>>(json);
        }

        public List<ProductCategorySDKModel> ReadCategorys()
        {
            var path = $"{AppPodDataDirectory}/Products/Categories.json";
            if (!File.Exists(path)) return null;
            string json = File.ReadAllText(path);
            return JsonConvert.DeserializeObject<List<ProductCategorySDKModel>>(json);

        }

        public List<TagSdkModel> ReadProductTags()
        {
            var path = $"{AppPodDataDirectory}/Products/ProductTags.json";
            if (!File.Exists(path)) return null;
            string json = File.ReadAllText(path);
            return JsonConvert.DeserializeObject<List<TagSdkModel>>(json);
        }

        public List<TagSdkModel> ReadAdTags()
        {
            var path = $"{AppPodDataDirectory}/Ads/AdTags.json";
            if (!File.Exists(path)) return null;
            string json = File.ReadAllText(path);
            return JsonConvert.DeserializeObject<List<TagSdkModel>>(json);
        }

        public List<TagSdkModel> ReadDeviceTags()
        {
            var path = $"{AppPodDataDirectory}/Products/DeviceTags.json";
            if (!File.Exists(path)) return null;
            string json = File.ReadAllText(path);
            return JsonConvert.DeserializeObject<List<TagSdkModel>>(json);
        }

        public List<StaffSdkModel> ReadStaffs()
        {
            var path = $"{AppPodDataDirectory}/Staffs/Staffs.json";
            if (!File.Exists(path)) return null;
            string json = File.ReadAllText(path);
            return JsonConvert.DeserializeObject<List<StaffSdkModel>>(json);
        }

        public List<AppInfo> ReadApps()
        {
            var path = $"{AppPodDataDirectory}/Apps/LocalApps.json";
            if (!File.Exists(path)) return null;
            string json = File.ReadAllText(path);
            return JsonConvert.DeserializeObject<List<AppInfo>>(json);
        }

        public List<DeviceSoftwareSdkModel> ReadDeviceSoftwares()
        {
            var path = $"{AppPodDataDirectory}/Apps/Apps.json";
            if (!File.Exists(path)) return null;
            string json = File.ReadAllText(path);
            return JsonConvert.DeserializeObject<List<DeviceSoftwareSdkModel>>(json);
        }

        public DeviceAppPodVersionModel ReadDeviceAppPodVersion()
        {
            var path = $"{AppPodDataDirectory}/AppPodVersion.json";
            if (!File.Exists(path)) return null;
            string json = File.ReadAllText(path);
            return JsonConvert.DeserializeObject<List<DeviceAppPodVersionModel>>(json)?.FirstOrDefault();
        }

        public static async Task<string> ReadText(string filePath)
        {
            using (FileStream sourceStream = new FileStream(filePath,
                FileMode.Open, FileAccess.Read, FileShare.Read,
                bufferSize: 4096, useAsync: true))
            {
                StringBuilder sb = new StringBuilder();

                byte[] buffer = new byte[0x1000];
                int numRead;
                while ((numRead = await sourceStream.ReadAsync(buffer, 0, buffer.Length)) != 0)
                {
                    string text = Encoding.Unicode.GetString(buffer, 0, numRead);
                    sb.Append(text);
                }

                return sb.ToString();
            }
        }


        #endregion

        public bool Intialize()
        {
            Ads = ReadAds();
            Staffs = ReadStaffs();
            AdTags = ReadAdTags();
            ProductTags = ReadProductTags();
            DeviceTags = ReadDeviceTags();
            Products = ReadProducts();
            PCategories = ReadCategorys();
            BuildCategoryPaths();
            Coupons = ReadCoupons();
            Matches = ReadProductMatches();
            Likes = ReadProductLikes();
            Properties = ReadProperties();
            ProductComments = ReadProductComments();
            Brands = ReadBrands();
            ActivityGames = ReadActivityGames();
            AdAndAppTimelineSchedules = ReadAdAndAppTimelineSchedules();
            Metas = ReadMetas();
            DateMetas = ReadDateMetas();
            Apps = ReadApps();
            DeviceSoftwares = ReadDeviceSoftwares();
            AppPodVersion = ReadDeviceAppPodVersion();
            

            return true;
        }

        public List<MetaPhysicsDto> ReadMetas()
        {
            var path = $"{AppPodDataDirectory}/Metas/Metas.json";
            if (!File.Exists(path)) return null;
            string json = File.ReadAllText(path);

            var metas = JsonConvert.DeserializeObject<List<MetaPhysicsDto>>(json);
            metas.ForEach(b => {
                b.LogoUrl = GetLocalImagePath(b.LogoUrl, "Metas");
                b.PicUrl = GetLocalImagePath(b.PicUrl, "Metas");
            });
            metas = metas.OrderBy(m => m.StartTime).ToList();
            return metas;
        }

        public List<DateMetaphysicsDto> ReadDateMetas()
        {
            var path = $"{AppPodDataDirectory}/Metas/DateMetas.json";
            if (!File.Exists(path)) return null;
            string json = File.ReadAllText(path);

            var dateMetas = JsonConvert.DeserializeObject<List<DateMetaphysicsDto>>(json);
            return dateMetas;
        }

        public List<ProductCommentModel> ReadProductComments()
        {
            var path = $"{AppPodDataDirectory}/Products/ProductComments.json";
            if (!File.Exists(path)) return null;
            string json = File.ReadAllText(path);

            var properties = JsonConvert.DeserializeObject<List<ProductCommentModel>>(json);
            return properties;
        }

        public List<BrandDto> ReadBrands()
        {
            var path = $"{AppPodDataDirectory}/Products/Brands.json";
            if (!File.Exists(path)) return null;
            string json = File.ReadAllText(path);

            var brands = JsonConvert.DeserializeObject<List<BrandDto>>(json);
            brands = brands.OrderBy(b => b.OrderNumber).ToList();
            brands.ForEach(b => {
                b.LogoUrl = GetLocalImagePath(b.LogoUrl, "Products");
                b.ImageUrl = GetLocalImagePath(b.ImageUrl, "Products");
                b.ItemImagesOrVideos.ForEach(item => {
                    item.FileUrl = GetLocalImagePath(item.FileUrl, "Products");
                });
            });
            return brands;
        }

        public List<ActivityGameDto> ReadActivityGames()
        {
            var path = $"{AppPodDataDirectory}/Products/ActivityGames.json";
            if (!File.Exists(path)) return null;
            string json = File.ReadAllText(path);
            var games = JsonConvert.DeserializeObject<List<ActivityGameDto>>(json);
            return games;
        }

        public List<AdAndAppTimelineScheduleViewModel> ReadAdAndAppTimelineSchedules()
        {
            var path = $"{AppPodDataDirectory}/AdAndApp_Timelines.json";
            if (!File.Exists(path)) return null;
            string json = File.ReadAllText(path);
            var timelines = JsonConvert.DeserializeObject<List<AdAndAppTimelineScheduleViewModel>>(json);
            return timelines;
        }


        public void BuildCategoryPaths()
        {
            List<ProductCategorySDKModel> categoriesWithPath = new List<ProductCategorySDKModel>();
            //读取根分类
            var roots = GetRootCategories();
            Stack<ProductCategorySDKModel> stack = new Stack<ProductCategorySDKModel>();
            foreach (var root in roots)
            {
                stack.Push(root);
                PCategories.Remove(root);
                //使用队列从根分类逐级读取子分类
                while (true)
                {
                    var topItem = stack.Peek();
                    if (topItem.Ids == null)
                        topItem.Ids = new List<int> { topItem.Id };
                    var child = PCategories.FirstOrDefault(p => p.ParentCategoryId == topItem.Id && p.ParentCategoryId != p.Id);
                    if (child != null)
                    {
                        stack.Push(child);
                        //child.Ids = new List<int> { child.Id };
                        PCategories.Remove(child);
                    }
                    else
                    {
                        var popItem = stack.Pop();
                        categoriesWithPath.Add(popItem);
                        if (stack.Count == 0)
                            break;
                        topItem = stack.Peek();
                        //topItem.Ids.Add(popItem.Id);
                        topItem.Ids.AddRange(popItem.Ids);
                    }
                }
            }

            PCategories = categoriesWithPath;
        }

        public List<ShowProductInfo> DistinctShowProducts(ProductSdkModel prod, long exceptSkuId = -1)
        {
            if (prod == null) return null;
            if (prod.PropImgs != null && prod.PropImgs.Count() > 0)
            {
                var showProducts = new List<ShowProductInfo>();
                foreach (var pImg in prod.PropImgs)
                {
                    var keyProps = pImg.PropertyName;
                    var firstSku = prod.Skus.AsQueryable().FirstOrDefault(s => s.PropsName.Contains(keyProps));
                    if (firstSku != null)
                    {
                        if (firstSku.Id != exceptSkuId)
                        {
                            showProducts.Add(new ShowProductInfo
                            {
                                Id = firstSku.Id,
                                ImageUrl = GetLocalImagePath(pImg.ImageUrl ?? firstSku.PicUrl, "Products"),
                                Quantity = firstSku.Quantity,
                                Name = firstSku.Title,
                                Price = firstSku.Price,
                                PromPrice = firstSku.PromPrice,
                                Type = ProductType.Sku,
                                PropsName = pImg.PropertyName
                            });
                        }
                    }
                }
                return showProducts;
            }
            return null;
        }

        public List<ShowProductInfo> FindSimilar(ShowProductInfo productInfo, bool useSameSpu = true, bool useSameCategories = false)
        {
            //todo:Qu.
            if (productInfo == null) return null;
            List<ShowProductInfo> similarSkus = new List<ShowProductInfo>();
            if (useSameSpu == false && useSameCategories == false)
            {
                similarSkus.Add(productInfo);
                return similarSkus;
            }
            if (productInfo.Type == ProductType.Product)
            {
                if (useSameSpu)
                {
                    var product = FindByShowProduct(productInfo);
                    similarSkus = DistinctShowProducts(product, productInfo.Id);
                    if (similarSkus == null) similarSkus = new List<ShowProductInfo>();
                    if (similarSkus.Count == 0)
                    {
                        similarSkus.Insert(0, productInfo);
                    }
                }
            }
            if (productInfo.Type == ProductType.Sku)
            {
                if (useSameSpu)      
                {
                    var spu = Products?.FirstOrDefault(p => p.Skus.Any(s => s.Id == productInfo.Id));
                    similarSkus = DistinctShowProducts(spu, productInfo.Id);
                    if (similarSkus == null) similarSkus = new List<ShowProductInfo>();
                    similarSkus.Insert(0, productInfo);
                }
            }
            return similarSkus;
        }

        public PropertyInfo GetKeyPropertyInfoInSkus(ShowProductInfo info)
        {
            var productSdkModel = FindByShowProduct(info);
            return GetKeyPropertyInfoInSkus(productSdkModel);
        }
        public PropertyInfo GetKeyPropertyInfoInSkus(ProductSdkModel product)
        {
            if (product == null || product.PropImgs == null || product.PropImgs.Count() == 0) return null;
            PropertyInfo propInfo = null;
            foreach (var propImg in product.PropImgs)
            {
                if (product.Skus != null)
                {
                    var first = product.Skus.FirstOrDefault(s => s.PropsName.Contains(propImg.PropertyName));
                    if (first != null)
                    {
                        var propName = propImg.PropertyName;
                        var info = GetPropertyName(propName);
                        if (info.Key == null) continue;
                        if (propInfo == null)
                        {
                            propInfo = new PropertyInfo { IsKey = true, Name = info.Key };
                            propInfo.Values.Add(new PropertyValueInfo { Name = info.Value, ImageUrl = propImg.ImageUrl, Sku = first });
                        }
                        else
                        {
                            if (propInfo.Name == info.Key)
                            {
                                var existedValue = propInfo.Values.Find(v => v.Name == info.Value);
                                if (existedValue != null) continue;
                                propInfo.Values.Add(new PropertyValueInfo { Name = info.Value, ImageUrl = propImg.ImageUrl, Sku = first });
                            }
                        }
                    }
                }
            }
            return propInfo;
        }

        public (string Key, string Value) GetPropertyName(string properties)
        {
            if (string.IsNullOrEmpty(properties)) return (null, null);
            var values = properties.Split(new string[] { ":" }, StringSplitOptions.RemoveEmptyEntries);
            if (values == null || values.Length != 4) return (null, null);
            if (values != null && values.Length == 4)
            {
                return (values[2], values[3]);
            }
            return (null, null);
        }

        public Dictionary<string, string> GetPropertyNames(string properties)
        {
            if (string.IsNullOrEmpty(properties)) return null;
            var props = properties.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
            if (props == null) return null;
            var keyValues = new Dictionary<string, string>();
            foreach (var prop in props)
            {
                var values = prop.Split(new string[] { ":" }, StringSplitOptions.RemoveEmptyEntries);
                if (values == null || values.Length != 4) continue;
                keyValues.Add(values[2], values[3]);
            }
            return keyValues;
        }

        public List<PropertyInfo> GetPropertyInfosInSkus(ProductSdkModel product)
        {

            if (product == null || product.Skus == null || product.Skus.Count() == 0) return null;
            var pInfos = new List<PropertyInfo>();
            var keyPInfo = GetKeyPropertyInfoInSkus(product);
            foreach (var sku in product.Skus)
            {
                var props = GetPropertyNames(sku.PropsName);
                foreach (var prop in props)
                {
                    var info = pInfos.Find(p => p.Name == prop.Key);
                    if (info == null)
                    {
                        info = new PropertyInfo { Name = prop.Key };
                        pInfos.Add(info);
                    }
                    if (!info.Values.Any(p => p.Name == prop.Value))
                    {
                        info.Values.Add(new PropertyValueInfo { Name = prop.Value });
                    }
                }
            }
            if (keyPInfo != null)
            {
                var key = pInfos.Find(p => p.Name == keyPInfo.Name);
                if (key != null)
                {
                    pInfos.Remove(key);
                    pInfos.Insert(0, keyPInfo);
                }
            }
            return pInfos;
        }

        public List<PropertyInfo> GetPropertyInfosInSkus(ShowProductInfo product)
        {
            var productSdkInfo = FindByShowProduct(product);
            return GetPropertyInfosInSkus(productSdkInfo);
        }

        public string GetStoreType()
        {
            if (DeviceSetting == null)
                return "Taobao";
            if (DeviceSetting.OnlineTrafficTarget == null)
                return "官方电商";
            return DeviceSetting.OnlineTrafficTarget.ToString();
        }

        /// <summary>
        /// nameValue must like that, 颜色:红色
        /// </summary>
        /// <param name="nameValues"></param>
        /// <returns></returns>
        public List<PropertyInfo> FindReminderAvailablePropertiesInSkus(ShowProductInfo showProduct, params string[] nameValues)
        {
            var productSdkModel = FindByShowProduct(showProduct);
            if (productSdkModel == null || productSdkModel.Skus == null || productSdkModel.Skus.Count() == 0 || nameValues.Length == 0) return null;
            var pInfos = new List<PropertyInfo>();
            List<SkuSdkModel> includeSkus = null;

            includeSkus = productSdkModel.Skus.Where(s => ContainsAll(s.PropsName, nameValues)).ToList();

            foreach (var sku in includeSkus)
            {
                var props = GetPropertyNames(sku.PropsName);
                foreach (var prop in props)
                {
                    var info = pInfos.Find(p => p.Name == prop.Key);
                    if (info == null)
                    {
                        info = new PropertyInfo { Name = prop.Key };
                        pInfos.Add(info);
                    }
                    if (!info.Values.Any(p => p.Name == prop.Value))
                    {
                        info.Values.Add(new PropertyValueInfo { Name = prop.Value });
                    }
                }
            }
            foreach (var nameValue in nameValues)
            {
                var keyValueArrary = nameValue.Split(new string[] { ":" }, StringSplitOptions.RemoveEmptyEntries);
                if (keyValueArrary.Length != 2) continue;
                var key = keyValueArrary[0];
                var value = keyValueArrary[1];
                var existedPInfo = pInfos.Find(p => p.Name == key);
                if (existedPInfo != null) pInfos.Remove(existedPInfo);
            }
            return pInfos;
        }


        public static bool ContainsAll(string propNames, string[] keyValues)
        {
            if (string.IsNullOrEmpty(propNames) || keyValues.Length == 0) return false;
            foreach (var value in keyValues)
            {
                if (!propNames.Contains(value)) return false;
            }
            return true;
        }

        public SkuSdkModel GetSkuSdkModelByShowProduct(ShowProductInfo showProductInfo)
        {
            if (showProductInfo == null || showProductInfo.Type == ProductType.Product) return null;
            var spu = Products?.FirstOrDefault(p => p.Skus.Any(s => s.Id == showProductInfo.Id));
            return spu.Skus.FirstOrDefault(s => s.Id == showProductInfo.Id);
        }

        public List<ShowProductInfo> QueryShowProductsByProperties(IDictionary<string, string> keyValues)
        {
            return null;
        }

        public async Task<List<ProductCommentModel>> GetProductComments(long productId)
        {
            if (ProductComments == null)
                return new List<ProductCommentModel>();
            return await Task.Factory.StartNew(() => {
                return ProductComments.Where(p => p.ProductId == productId).ToList();
            });
        }

        public IEnumerable<ShowProductInfo> GetBrandProducts(long brandId)
        {
            if (mShowProducts == null)
                mShowProducts = QueryShowProducts(false);
            return mShowProducts.Where(p => p.Product.BrandId.Value == brandId);
        }

        public IEnumerable<ProductCategorySDKModel> GetBrandCategories(long brandId)
        {
            //获取品牌的所有商品
            var categorys = Products.Where(p => p.BrandId == brandId).SelectMany(b => b.CategoryIds).Distinct();
            //过滤掉根分类

            return PCategories.Where(category => categorys.Any(id => id == category.Id) && category.ParentCategoryId != 0 && category.ParentCategoryId != category.Id);
        }

        public IEnumerable<ShowProductInfo> QueryProducts(long brandId, int categoryId)
        {
            ProductCategorySDKModel category = null;
            if (categoryId > 0)
            {
                category = PCategories.FirstOrDefault(c => c.Id == categoryId);
            }
            return mShowProducts.Where(p =>  categoryId <= 0 || (category != null && p.Product.CategoryIds.Intersect(category.Ids).Count() > 0))
                                .Where(p => brandId <= 0 || p.Product.BrandId == brandId);
        }
        public IEnumerable<BrandDto> GetBrandsByMainCategory(int categoryId)
        {
            var mainCategory = PCategories.FirstOrDefault(c => c.Id == categoryId);
            var brandIds = Products.Where(p => p.CategoryIds.Intersect(mainCategory.Ids).Count() > 0).Select(p => p.BrandId).Distinct();
            return Brands.Where(b => brandIds.Any(id => id == b.Id));
        }

        public const string AstroString = "星座,astro";
        public IEnumerable<MetaPhysicsDto> GetAllAstro()
        {
            if (Metas == null || Metas.Count == 0) return null;
            return Metas.Where(m => AstroString.Contains(m.Type.Name)).ToList();
        }

        public DateMetaphysicsDto GetNowOrLatestLuckyByMetaId(long metaId)
        {
            if (DateMetas == null || DateMetas.Count == 0) return null;
            return DateMetas.Where(m => m.MetaphysicsId == metaId).OrderByDescending(m => m.Date).FirstOrDefault();
        }

        public IEnumerable<AdsSdkModel> FindAdsByTagName(string tagName)
        {
            if (AdTags == null)
                return null;
            var tag = AdTags?.FirstOrDefault(t => t.Value == tagName);
            if (tag == null)
            {
                return Enumerable.Empty<AdsSdkModel>();
            }
            var ads = Ads.Where(t => t.TagIds.Contains(tag.Id)).OrderBy(t => t.OrderNumber);
            ads.ForEach((a) => {
                a.FileUrl = a.GetLocalFile();
            });
            return ads;
        }

        public void RemoveFrontBrandName()
        {
            var showProducts = QueryShowProducts(false);
            showProducts.ForEach(p => {
                string brandName = p.BrandName;
                if (brandName != null)
                {
                    if (p.ProductName.Substring(0, p.BrandName.Length) == brandName)
                    {
                        p.ProductName = p.ProductName.Substring(brandName.Length).TrimStart();
                        p.Product.Title = p.ProductName;
                    }
                }
            });
        }

        public void RemovePrefixText(string text)
        {
            var showProducts = QueryShowProducts(false);
            showProducts.ForEach(p => {
                var keywords = text.Split(new char[] { ';' });
                foreach (var keyword in keywords)
                {
                    if (string.IsNullOrEmpty(keyword))
                        continue;
                    if ( p.Product.Title.StartsWith(keyword))
                    {
                        p.ProductName = p.ProductName.Substring(keyword.Length).TrimStart();
                        p.Name = p.ProductName;
                        p.Product.Title = p.ProductName;
                    }
                    if (p.Product.Skus != null)
                    {
                        foreach (var sku in p.Product.Skus)
                        {
                            if (sku.Title.StartsWith(keyword))
                            {
                                sku.Title = sku.Title.Substring(keyword.Length).TrimStart();
                            }

                        }
                    }

                }

            });
        }

        public void FindAllMatch()
        {
            if (MatchedProducts == null)
                MatchedProducts = new Dictionary<long, IEnumerable<ShowProductInfo>>();
            foreach (var matchGroup in Matches)
            {
                var mainItem = matchGroup.MatchItems.FirstOrDefault(m => m.IsMain == true);
                if (mainItem == null)
                    continue;
                var mainProduct = Products.FirstOrDefault(p => p.Skus.Any(s => s.Id == mainItem.SkuId));
                if (mainProduct == null)
                    continue;
                var showProducts = new List<ShowProductInfo>();
                if (MatchedProducts.ContainsKey(mainProduct.Id))
                    continue;
                MatchedProducts.Add(mainProduct.Id, showProducts);
                foreach (var item in matchGroup.MatchItems)
                {
                    var product = Products.FirstOrDefault(p => p.Skus.Any(s => s.Id == item.SkuId));
                    if (product == null)
                        continue;
                   var showProduct =  mShowProducts.FirstOrDefault(p => p.Product == product);
                    if (showProduct == null)
                        continue;
                    showProducts.Add(showProduct);
                }
            }
        }

        public ShowProductInfo GetShowProductByOutId(string outId)
        {
            var product = Products.FirstOrDefault(p => p.Skus.Any(s => s.OuterId == outId));
            if (product == null)
                return null;
            var sku = product.Skus.FirstOrDefault(s => s.OuterId == outId);
            if (sku == null)
                return null;
            return new ShowProductInfo
            {
                Id = sku.Id,
                ImageUrl = GetLocalImagePath(sku.PicUrl, "Products"),
                Quantity = product.Num,
                Name = sku.Title,
                BrandName =  Brands.FirstOrDefault(b => b.Id == product.BrandId)?.Name,
                ProductName = product.Title,
                Price = sku.Price,
                PromPrice = sku.PromPrice,
                //QrcodeUrl = prod.OnlineStoreInfos.FirstOrDefault(s => s.OnlineStoreType == storeType)?.Qrcode,
                Type = ProductType.Sku,
                TagIconUrl = FindTagIcon(product.TagIds, SvcType.Product),
                Product = product,
                
            };
        }

        public ShowProductInfo GetShowProductBySku(long skuId)
        {
            var product = Products.FirstOrDefault(p => p.Skus.Any(s => s.Id == skuId));
            if (product == null)
                return null;
            var sku = product.Skus.FirstOrDefault(s => s.Id == skuId);
            if (sku == null)
                return null;
            return new ShowProductInfo
            {
                Id = sku.Id,
                ImageUrl = GetLocalImagePath(sku.PicUrl, "Products"),
                Quantity = product.Num,
                Name = sku.Title,
                BrandName = Brands.FirstOrDefault(b => b.Id == product.BrandId)?.Name,
                ProductName = product.Title,
                Price = sku.Price,
                PromPrice = sku.PromPrice,
                //QrcodeUrl = prod.OnlineStoreInfos.FirstOrDefault(s => s.OnlineStoreType == storeType)?.Qrcode,
                Type = ProductType.Sku,
                TagIconUrl = FindTagIcon(product.TagIds, SvcType.Product),
                Product = product
            };
        }

        public void ReadClickCounts(List<ClickInfo> clickInfoData)
        {
            var clickDic = clickInfoData.ToDictionary(x => long.Parse(x.ThingId));
            foreach (var product in mShowProducts)
            {
                if (clickDic.ContainsKey(product.Id))
                {
                    var clickInfo = clickDic[product.Id];
                    product.ClickCount = clickInfo.ClickCount;
                }
            }
        }

        public void ReadLikeClickCounts(List<ClickInfo> clickInfoData)
        {
            var clickDic = clickInfoData.ToDictionary(x => long.Parse(x.ThingId));
            foreach (var product in mShowProducts)
            {
                if (clickDic.ContainsKey(product.Id))
                {
                    var clickInfo = clickDic[product.Id];
                    product.LikeClickCount = clickInfo.ClickCount;
                }
            }
        }

        public void ReadAllClickCounts(List<ClickInfo> clickInfoData)
        {
            var clickDic = clickInfoData.ToDictionary(x => long.Parse(x.ThingId));
            foreach (var product in mShowProducts)
            {
                if (clickDic.ContainsKey(product.Id))
                {
                    var clickInfo = clickDic[product.Id];
                    product.TotalClickCouont = clickInfo.ClickCount;
                }
            }
        }

        public void ReadLikeCounts(List<ClickInfo> clickInfoData)
        {
            var clickDic = clickInfoData.ToDictionary(x => long.Parse(x.ThingId));
            foreach (var product in mShowProducts)
            {
                if (clickDic.ContainsKey(product.Id))
                {
                    var clickInfo = clickDic[product.Id];
                    product.Product.LikeCount = clickInfo.ClickCount;
                }
            }
        }

        public void SetKeyword(string keyword)
        {
            mShowProducts.ForEach((p) => {
                if (p.Product.Title.Contains(keyword))
                {
                    p.Product.Keywords = keyword;
                }
            });
        }

        public IEnumerable<ShowProductInfo> QueryByKeyword(string keyword)
        {
            return mShowProducts.Where(p => p.Product.Keywords == keyword);
        }
    }
}
