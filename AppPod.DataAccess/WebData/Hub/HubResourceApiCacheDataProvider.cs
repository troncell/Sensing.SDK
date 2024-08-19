using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AppPod.DataAccess.WebData.Store.Do;
using LogService;
using Sensing.Device.SDK.Dto;
using Sensing.SDK;
using Sensing.SDK.Contract;

namespace AppPod.DataAccess.WebData.Store
{
    public class HubResourceApiCacheDataProvider
    {
        protected readonly IBizLogger logger = ServerLogFactory.GetLogger(typeof(HubResourceApiCacheDataProvider));

   
        private int TotalDownloadCount = 0;

        public HubResourceApiCacheDataProvider()
        {
        }

        
        private string AbsolutePath(string path, string baseUrl)
        {
            return path == null || path.Contains("alicdn.com") || path.ToLower().StartsWith("http") ? path : baseUrl + path;
        }
        
        public virtual IList<DownloadLink> ExtractProductLinks(List<ProductSdkModel> productsData,string baseUrl)
        {
            if (productsData == null) return null;
            List<DownloadLink> links = new List<DownloadLink>();
            foreach (var item in productsData)
            {
                if (item.PicUrl != null)
                {
                    var absolute = AbsolutePath(item.PicUrl, baseUrl);

                    if (!links.Any(link => link.RelativeFileName == absolute))
                    {
                        links.Add(new DownloadLink { Host = baseUrl, RelativeFileName = absolute, Type = "product" });
                    }
                }
                if (item.ItemImagesOrVideos != null)
                {
                    foreach (var res in item.ItemImagesOrVideos)
                    {
                        var absolute = AbsolutePath(res.FileUrl, baseUrl);
                        if (!links.Any(link => link.RelativeFileName == absolute))
                        {
                            links.Add(new DownloadLink { Host = baseUrl, RelativeFileName = absolute, Type = "product" });
                        }
                    }
                }

                if (item.PropImgs != null)
                {
                    foreach (var pImg in item.PropImgs)
                    {
                        var absolute = AbsolutePath(pImg.ImageUrl, baseUrl);
                        if (!links.Any(link => link.RelativeFileName == absolute))
                        {
                            links.Add(new DownloadLink { Host = baseUrl, RelativeFileName = absolute, Type = "product" });
                        }
                    }
                }
                var skus = item.Skus;
                if (skus != null)
                {
                    foreach (var sku in skus)
                    {
                        if (sku != null)
                        {
                            if (sku.ItemImagesOrVideos != null)
                            {
                                foreach (var res in sku.ItemImagesOrVideos)
                                {
                                    var absolute = AbsolutePath(res.FileUrl, baseUrl);
                                    if (!links.Any(link => link.RelativeFileName == absolute))
                                    {
                                        links.Add(new DownloadLink { Host = baseUrl, RelativeFileName = absolute, Type = "sku" });
                                    }
                                }
                            }
                            if (sku.OnlineStoreInfos != null)
                            {
                                foreach (var res in sku.OnlineStoreInfos)
                                {
                                    if (string.IsNullOrEmpty(res?.Qrcode))
                                        continue;
                                    if (res.Qrcode.EndsWith(".png") || res.Qrcode.EndsWith(".jpg"))
                                    {
                                        var absolute = AbsolutePath(res.Qrcode, baseUrl);
                                        if (!links.Any(link => link.RelativeFileName == absolute))
                                        {
                                            links.Add(new DownloadLink { Host = baseUrl, RelativeFileName = absolute, Type = "product" });
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            TotalDownloadCount += links.Count;
            logger.Debug($"Products Download Source Total is {TotalDownloadCount}");
            return links;
        }

        public virtual IList<DownloadLink> ExtractMetaPhysicsLinks(List<MetaPhysicsDto> metaPhysicsDtoList,string baseUrl)
        {
            if (metaPhysicsDtoList == null)
                return (IList<DownloadLink>) null;
            baseUrl = SensingWebClient.RecommendApiHost;
            List<DownloadLink> source = new List<DownloadLink>();
            foreach (MetaPhysicsDto metaPhysicsDto in metaPhysicsDtoList)
            {
                MetaPhysicsDto item = metaPhysicsDto;
                if (item.LogoUrl != null && !source.Any<DownloadLink>((Func<DownloadLink, bool>) (link => link.RelativeFileName == item.LogoUrl)))
                    source.Add(new DownloadLink()
                    {
                        Host = baseUrl,
                        RelativeFileName = item.LogoUrl,
                        Type = "meta"
                    });
                if (item.PicUrl != null && !source.Any<DownloadLink>((Func<DownloadLink, bool>) (link => link.RelativeFileName == item.PicUrl)))
                    source.Add(new DownloadLink()
                    {
                        Host = baseUrl,
                        RelativeFileName = item.PicUrl,
                        Type = "meta"
                    });
            }
            this.TotalDownloadCount += source.Count;
            this.logger.Debug((object) string.Format("Recommend Download Source Total is {0}", (object) this.TotalDownloadCount));
            return (IList<DownloadLink>) source;
        }
        
        public virtual IList<DownloadLink> ExtractBrandsLinks(List<BrandDto> brands,string baseUrl)
        {
            if (brands == null) return null;
            List<DownloadLink> links = new List<DownloadLink>();
            foreach (var item in brands)
            {
                if (item.LogoUrl != null)
                {
                    if (!links.Any(link => link.RelativeFileName == item.LogoUrl))
                    {
                        links.Add(new DownloadLink { Host = baseUrl, RelativeFileName = item.LogoUrl, Type = "Brands" });
                    }
                }
                if (item.ImageUrl != null)
                {
                    if (!links.Any(link => link.RelativeFileName == item.ImageUrl))
                    {
                        links.Add(new DownloadLink { Host = baseUrl, RelativeFileName = item.ImageUrl, Type = "Brands" });
                    }
                }
                if (item.ItemImagesOrVideos != null)
                {
                    foreach (var source in item.ItemImagesOrVideos)
                    {
                        if (!links.Any(link => link.RelativeFileName == source.FileUrl))
                        {
                            links.Add(new DownloadLink { Host = baseUrl, RelativeFileName = source.FileUrl, Type = "Brands" });
                        }
                    }
                }
            }

            TotalDownloadCount += links.Count;
            logger.Debug($"Brands Download Source Total is {links.Count}");
            return links;
        }
        
        public virtual IList<DownloadLink> ExtractCategoryLinks(List<ProductCategorySDKModel> categoriesData,string baseUrl)
        {
            if (categoriesData == null)
                return (IList<DownloadLink>) null;
            List<DownloadLink> source = new List<DownloadLink>();
            foreach (ProductCategorySDKModel categorySdkModel in categoriesData)
            {
                ProductCategorySDKModel item = categorySdkModel;
                if (!string.IsNullOrEmpty(item.IconUrl) && !source.Any<DownloadLink>((Func<DownloadLink, bool>) (link => link.RelativeFileName == item.IconUrl)))
                    source.Add(new DownloadLink()
                    {
                        Host = baseUrl,
                        RelativeFileName = item.IconUrl,
                        Type = "Category"
                    });
                if (!string.IsNullOrEmpty(item.ImageUrl) && !source.Any<DownloadLink>((Func<DownloadLink, bool>) (link => link.RelativeFileName == item.ImageUrl)))
                    source.Add(new DownloadLink()
                    {
                        Host = baseUrl,
                        RelativeFileName = item.ImageUrl,
                        Type = "Category"
                    });
            }
            this.TotalDownloadCount += source.Count;
            this.logger.Debug((object) string.Format("Categories Download Source Total is {0}", (object) source.Count));
            return (IList<DownloadLink>) source;
        }
        
        public virtual IList<DownloadLink> ExtractTagsLinks(List<TagSdkModel> tagsData,string baseUrl)
        {
            if (tagsData == null)
                return (IList<DownloadLink>) null;
            List<DownloadLink> source = new List<DownloadLink>();
            foreach (TagSdkModel tagSdkModel in tagsData)
            {
                TagSdkModel item = tagSdkModel;
                if (item.IconUrl != null && !source.Any<DownloadLink>((Func<DownloadLink, bool>) (link => link.RelativeFileName == item.IconUrl)))
                    source.Add(new DownloadLink()
                    {
                        Host = baseUrl,
                        RelativeFileName = item.IconUrl,
                        Type = "Category"
                    });
            }
            this.TotalDownloadCount += source.Count;
            this.logger.Debug((object) string.Format("Categories Download Source Total is {0}", (object) source.Count));
            return (IList<DownloadLink>) source;
        }
        public IList<DownloadLink> ExtractCoponsLinks(List<CouponViewModel> couponData, string serverBase)
        {
            if (couponData == null)
                return (IList<DownloadLink>) null;
            List<DownloadLink> source = new List<DownloadLink>();
            foreach (CouponViewModel couponViewModel in couponData)
            {
                CouponViewModel item = couponViewModel;
                if (item.Pictures != null && !source.Any<DownloadLink>((Func<DownloadLink, bool>) (link => link.RelativeFileName == item.Pictures)))
                    source.Add(new DownloadLink()
                    {
                        Host = serverBase,
                        RelativeFileName = item.Pictures,
                        Type = "Coupon"
                    });
            }
            this.TotalDownloadCount += source.Count;
            this.logger.Debug((object) string.Format("Copons Download Source Total is {0}", (object) this.TotalDownloadCount));
            return (IList<DownloadLink>) source;
        }
        public IList<DownloadLink> ExtractMatchLinks(List<MatchInfoViewModel> matchInfoViewModels,string baseUrl)
        {
            if (matchInfoViewModels == null)
                return (IList<DownloadLink>) null;
            List<DownloadLink> source = new List<DownloadLink>();
            foreach (MatchInfoViewModel matchInfoViewModel in matchInfoViewModels)
            {
                MatchInfoViewModel item = matchInfoViewModel;
                if (item.ShowImage != null && !source.Any<DownloadLink>((Func<DownloadLink, bool>) (link => link.RelativeFileName == item.ShowImage)))
                    source.Add(new DownloadLink()
                    {
                        Host = baseUrl,
                        RelativeFileName = item.ShowImage,
                        Type = "Match"
                    });
            }
            this.TotalDownloadCount += source.Count;
            this.logger.Debug((object) string.Format("Match Download Source Total is {0}", (object) this.TotalDownloadCount));
            return (IList<DownloadLink>) source;
        }
        
        public virtual IList<DownloadLink> ExtractAppsLinks(List<DeviceSoftwareSdkModel> appModels,string baseUrl)
        {
          if (appModels == null)
            return (IList<DownloadLink>) null;
          List<DownloadLink> source = new List<DownloadLink>();
          foreach (DeviceSoftwareSdkModel softwareSdkModel in appModels)
          {
            DeviceSoftwareSdkModel item = softwareSdkModel;
            if (item != null)
            {
              if (item.MaterialPacketUrl != null && !source.Any<DownloadLink>((Func<DownloadLink, bool>) (link => link.RelativeFileName == item.MaterialPacketUrl)))
                source.Add(new DownloadLink()
                {
                  Host = baseUrl,
                  RelativeFileName = item.MaterialPacketUrl,
                  Type = "apps"
                });
              if (item.Software != null)
              {
                if (!string.IsNullOrEmpty(item.Software.PackageUrl) && !source.Any<DownloadLink>((Func<DownloadLink, bool>) (link => link.RelativeFileName == item.Software.PackageUrl)))
                  source.Add(new DownloadLink()
                  {
                    Host = baseUrl,
                    RelativeFileName = item.Software.PackageUrl,
                    Type = "apps"
                  });
                if (!string.IsNullOrEmpty(item.Software.LogoUrl) && !source.Any<DownloadLink>((Func<DownloadLink, bool>) (link => link.RelativeFileName == item.Software.LogoUrl)))
                  source.Add(new DownloadLink()
                  {
                    Host = baseUrl,
                    RelativeFileName = item.Software.LogoUrl,
                    Type = "apps"
                  });
                if (!string.IsNullOrEmpty(item.Software.LargeImageUrl) && !source.Any<DownloadLink>((Func<DownloadLink, bool>) (link => link.RelativeFileName == item.Software.LargeImageUrl)))
                  source.Add(new DownloadLink()
                  {
                    Host = baseUrl,
                    RelativeFileName = item.Software.LargeImageUrl,
                    Type = "apps"
                  });
              }
              if (item.TenantAppSetting != null && !string.IsNullOrEmpty(item.TenantAppSetting.MaterialPacketUrl) && !source.Any<DownloadLink>((Func<DownloadLink, bool>) (link => link.RelativeFileName == item.TenantAppSetting.MaterialPacketUrl)))
                source.Add(new DownloadLink()
                {
                  Host = baseUrl,
                  RelativeFileName = item.TenantAppSetting.MaterialPacketUrl,
                  Type = "apps"
                });
            }
          }
          this.TotalDownloadCount = source.Count;
          this.logger.Debug((object) string.Format("Ads Download Source Total is {0}", (object) this.TotalDownloadCount));
          return (IList<DownloadLink>) source;
        }

        public virtual IList<DownloadLink> ExtractDeviceTagsLinks(List<TagSdkModel> deviceTagsData,string baseUrl)
        {
            if (deviceTagsData == null)
                return (IList<DownloadLink>) null;
            List<DownloadLink> source = new List<DownloadLink>();
            foreach (TagSdkModel tagSdkModel in deviceTagsData)
            {
                TagSdkModel item = tagSdkModel;
                if (item.IconUrl != null && !source.Any<DownloadLink>((Func<DownloadLink, bool>) (link => link.RelativeFileName == item.IconUrl)))
                    source.Add(new DownloadLink()
                    {
                        Host = baseUrl,
                        RelativeFileName = item.IconUrl,
                        Type = "Category"
                    });
            }
            this.TotalDownloadCount += source.Count;
            this.logger.Debug((object) string.Format("Categories Download Source Total is {0}", (object) source.Count));
            return (IList<DownloadLink>) source;
        }
        
        public virtual IList<DownloadLink> ExtractHallAreaLinks(List<HallAreaDto> hallAreaDtos,string baseUrl)
        {
            if (hallAreaDtos == null)
                return (IList<DownloadLink>) null;
            List<DownloadLink> source = new List<DownloadLink>();
            foreach (HallAreaDto dto in hallAreaDtos)
            {
                HallAreaDto item = dto;
                if (item.IconUrl != null && !source.Any<DownloadLink>((Func<DownloadLink, bool>) (link => link.RelativeFileName == item.IconUrl)))
                    source.Add(new DownloadLink()
                    {
                        Host = baseUrl,
                        RelativeFileName = item.IconUrl,
                        Type = "HallArea"
                    });
                if (item.BackgroundUrl != null && !source.Any<DownloadLink>((Func<DownloadLink, bool>) (link => link.RelativeFileName == item.BackgroundUrl)))
                    source.Add(new DownloadLink()
                    {
                        Host = baseUrl,
                        RelativeFileName = item.BackgroundUrl,
                        Type = "HallArea"
                    });
                foreach (var itemHallAreaControlDto in item.HallAreaControlDtos)
                {
                    if (itemHallAreaControlDto.IconUrl != null && !source.Any<DownloadLink>((Func<DownloadLink, bool>) (link => link.RelativeFileName == itemHallAreaControlDto.IconUrl)))
                    {
                        source.Add(new DownloadLink()
                        {
                            Host = baseUrl,
                            RelativeFileName = itemHallAreaControlDto.IconUrl,
                            Type = "HallArea"
                        });
                    }
                }
                
            }
            this.TotalDownloadCount += source.Count;
            this.logger.Debug((object) string.Format("HallArea Download Source Total is {0}", (object) source.Count));
            return (IList<DownloadLink>) source;
        }
        
        public virtual IList<DownloadLink> ExtractStoreLinks(List<StoreItem> storeItems,string baseUrl)
        {
            if (storeItems == null)
                return (IList<DownloadLink>) null;
            List<DownloadLink> source = new List<DownloadLink>();
            foreach (StoreItem dto in storeItems)
            {
                StoreItem item = dto;
                if (item.ImgUrl != null && !source.Any<DownloadLink>((Func<DownloadLink, bool>) (link => link.RelativeFileName == item.ImgUrl)))
                    source.Add(new DownloadLink()
                    {
                        Host = baseUrl,
                        RelativeFileName = item.ImgUrl,
                        Type = "Store"
                    });
            }
            this.TotalDownloadCount += source.Count;
            this.logger.Debug((object) string.Format("Store Download Source Total is {0}", (object) source.Count));
            return (IList<DownloadLink>) source;
        }
        
        public static string ExtractSchema(string fileName)
        {
            if (fileName == null) return null;
            if (fileName.Contains("&") || fileName.Contains("?"))
            {
                return ExtractSchemaMd5(fileName);
            }
            string fileNamePath = fileName;
            if (fileName.StartsWith("http", true, CultureInfo.CurrentCulture))
            {
                var uri = new Uri(fileName).LocalPath;
                fileNamePath = SanitizeFileName(uri);
            }
            return ChangeFileName(fileNamePath);
        }
        public static string ChangeFileName(string fileNamePath)
        {
            if (IsSS2File(fileNamePath))
            {
                var fileName = fileNamePath.Substring(0, fileNamePath.Length - 1 - 4);
                return fileName + ".jpg";
            }
            return fileNamePath;
        }
        public static bool IsSS2File(string f)
        {
            return f != null &&
                   f.EndsWith(".SS2", StringComparison.Ordinal);
        }
        public static string ExtractSchemaMd5(string fileName)
        {
            if (fileName == null) return null;
            using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
            {
                byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(fileName);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                // Convert the byte array to hexadecimal string
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("X2"));
                }
                return sb.ToString() + ".jpg";
            }
        }
        public static string SanitizeFileName(string path)
        {
            int index = path.LastIndexOf("/");
            string filename = path.Substring(index + 1);
            string regex = String.Format(@"[{0}]+", Regex.Escape(new string(Path.GetInvalidFileNameChars())));
            filename = Regex.Replace(filename, regex, "_");
            return Path.Combine(path.Substring(0, index + 1), filename);
        }

    }
}
