using AppPod.DataAccess.Models;
using Sensing.SDK.Contract;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AppPod.DataAccess
{
    public static class Extensions
    {
        public static string GetLocalCategoryImage(this ProductCategorySDKModel category)
        {
            if (category == null || string.IsNullOrEmpty(category.ImageUrl)) return null;
            var localPath = ExtractSchema(category.ImageUrl);
            return $"{SensingDataAccess.AppPodDataDirectory}\\Products\\res\\{localPath}";
        }

        public static string GetLocalCategoryIcon(this ProductCategorySDKModel category)
        {
            if (category == null || string.IsNullOrEmpty(category.IconUrl)) return null;
            var localPath = ExtractSchema(category.IconUrl);
            return $"{SensingDataAccess.AppPodDataDirectory}\\Products\\res\\{localPath}";
        }

        public static string GetLocalImage(this ShowProductInfo productInfo)
        {
            if (productInfo == null || string.IsNullOrEmpty(productInfo.ImageUrl)) return null;
            var localPath = ExtractSchema(productInfo.ImageUrl);
            return $"{SensingDataAccess.AppPodDataDirectory}\\Products\\res\\{localPath}";
        }

        public static string GetLocalImage(this StaffSdkModel staffSdkModel)
        {
            //todo:william//
            //if (staffSdkModel == null || string.IsNullOrEmpty(staffSdkModel.AvatarUrl)) return null;
            //var localPath = ExtractSchema(staffSdkModel.AvatarUrl);
            //return $"{SensingDataAccess.AppPodDataDirectory}\\Staffs\\res\\{localPath}";
            return null;
        }

        public static string GetLocalFile(this AdsSdkModel ads)
        {
            if (ads == null || string.IsNullOrEmpty(ads.FileUrl)) return null;
            var localPath = ExtractSchema(ads.FileUrl);
            return $"{SensingDataAccess.AppPodDataDirectory}\\ads\\res\\{localPath}";
        }





        public static ShowProductInfo ToShowProductInfo(this ProductSdkModel productInfo)
        {
            if (productInfo == null) return null;
            return new ShowProductInfo
            {
                Id = productInfo.Id,
                ImageUrl = productInfo.PicUrl,
                Name = productInfo.Title,
                Price = productInfo.Price,
                Quantity = productInfo.Num,
                Type = ProductType.Product
            };
        }

        public static string GetLocalFile(this PropertyValueInfo pValueInfo)
        {
            if (pValueInfo == null || string.IsNullOrEmpty(pValueInfo.ImageUrl)) return null;
            var localPath = ExtractSchema(pValueInfo.ImageUrl);
            return $"{SensingDataAccess.AppPodDataDirectory}\\products\\res\\{localPath}";
        }

        public static string GetLocalFilePath(this EntityFileSdkModel prodcutFileSdkModel)
        {
            if (prodcutFileSdkModel == null || string.IsNullOrEmpty(prodcutFileSdkModel.FileUrl)) return null;
            var localPath = ExtractSchema(prodcutFileSdkModel.FileUrl);
            return $"{SensingDataAccess.AppPodDataDirectory}\\products\\res\\{localPath}";
        }
        public static string GetLocalIconFile(this TagSdkModel tagSdkModel)
        {
            if (tagSdkModel == null || string.IsNullOrEmpty(tagSdkModel.IconUrl)) return null;
            var localPath = ExtractSchema(tagSdkModel.IconUrl);
            return $"{SensingDataAccess.AppPodDataDirectory}\\products\\res\\{localPath}";
        }

        public static List<ShowProductInfo> ToShowProductInfo(this List<ProductSdkModel> productInfos)
        {
            return productInfos.Select(p => new ShowProductInfo
            {
                Id = p.Id,
                ImageUrl = p.PicUrl,
                Name = p.Title,
                Price = p.Price,
                Quantity = p.Num,
                Type = ProductType.Product
            }).ToList();
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
    }
}
