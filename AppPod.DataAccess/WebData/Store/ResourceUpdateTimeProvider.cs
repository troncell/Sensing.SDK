using Newtonsoft.Json;
using Sensing.SDK;
using Sensing.SDK.Contract;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppPod.DataAccess.WebData.Store
{
    public class ResourceUpdateTimeProvider
    {
        private string _jsonFile;
        private SensingWebClient _webClient;
        public ResourceUpdateTimeProvider(SensingWebClient webclient, string productBaseFolder)
        {
            _jsonFile = Path.Combine(productBaseFolder, "ResourceUpdateTime.json");
            _webClient = webclient;
        }

        public async Task Intialize()
        {
            OnlineLastUpdateTime = await _webClient.GetLastUpdateTime();
            LocalLastUpdateTime = ReadDataFromJsonFile(_jsonFile);
            if (LocalLastUpdateTime == null)
            {
                LocalLastUpdateTime = new TableLastTimeDto();
            }
        }

        public void Reset()
        {
            OnlineLastUpdateTime = null;
        }

        public void Save()
        {
            if (LocalLastUpdateTime != null)
            {
                WriteDataToJsonFile(LocalLastUpdateTime, _jsonFile);
            }
        }

        private TableLastTimeDto ReadDataFromJsonFile(string jsonFile)
        {
            try
            {
                if (File.Exists(jsonFile))
                {
                    return JsonConvert.DeserializeObject<TableLastTimeDto>(File.ReadAllText(jsonFile));
                }
            }
            catch (Exception e)
            {

            }
            return null;
        }
        private void WriteDataToJsonFile(object data, string filePath)
        {
            if (data != null)
            {
                var jsonData = JsonConvert.SerializeObject(data);
                if (!string.IsNullOrEmpty(jsonData))
                {
                    using (StreamWriter file = new StreamWriter(filePath, false))
                    {
                        file.WriteAsync(jsonData).Wait();
                    }
                }
            }
        }

        public TableLastTimeDto OnlineLastUpdateTime { get; set; }
        public TableLastTimeDto LocalLastUpdateTime { get; set; }


        public bool AdsHasUpdate
        {
            get
            {
                return OnlineLastUpdateTime?.Ads == null || LocalLastUpdateTime?.Ads == null || OnlineLastUpdateTime.Ads > LocalLastUpdateTime.Ads;
            }
        }
        public bool AppsHasUpdate
        {
            get
            {
                return OnlineLastUpdateTime?.Apps == null || LocalLastUpdateTime?.Apps == null || OnlineLastUpdateTime.Apps > LocalLastUpdateTime.Apps;
            }
        }
        public bool ProductCategoriesHasUpdate
        {
            get
            {
                return OnlineLastUpdateTime?.ProductCategories == null || LocalLastUpdateTime?.ProductCategories == null || OnlineLastUpdateTime.ProductCategories > LocalLastUpdateTime.ProductCategories;
            }
        }
        public bool ProductsHasUpdate
        {
            get
            {
                return OnlineLastUpdateTime?.Products == null || LocalLastUpdateTime?.Products == null || OnlineLastUpdateTime.Products > LocalLastUpdateTime.Products;
            }
        }
        public bool ProductsSkuPropertyValuesHasUpdate
        {
            get
            {
                return ProductsHasUpdate || SkusHasUpdate || PropertyValuesHasUpdate;
            }
        }
        public bool SkusHasUpdate
        {
            get
            {
                return OnlineLastUpdateTime?.Skus == null || LocalLastUpdateTime?.Skus == null || OnlineLastUpdateTime.Skus > LocalLastUpdateTime.Skus;
            }
        }
        public bool TagsHasUpdate
        {
            get
            {
                return OnlineLastUpdateTime?.Tags == null || LocalLastUpdateTime?.Tags == null || OnlineLastUpdateTime.Tags > LocalLastUpdateTime.Tags;
            }
        }
        public bool ProductCommentsHasUpdate
        {
            get
            {
                return OnlineLastUpdateTime?.ProductComments == null || LocalLastUpdateTime?.ProductComments == null || OnlineLastUpdateTime.ProductComments > LocalLastUpdateTime.ProductComments;
            }
        }
        public bool PropertiesHasUpdate
        {
            get
            {
                return OnlineLastUpdateTime?.Properties == null || LocalLastUpdateTime?.Properties == null || OnlineLastUpdateTime.Properties > LocalLastUpdateTime.Properties;
            }
        }
        public bool PropertyValuesHasUpdate
        {
            get
            {
                return OnlineLastUpdateTime?.PropertyValues == null || LocalLastUpdateTime?.PropertyValues == null || OnlineLastUpdateTime.PropertyValues > LocalLastUpdateTime.PropertyValues;
            }
        }
        public bool CouponsHasUpdate
        {
            get
            {
                return OnlineLastUpdateTime?.Coupons == null || LocalLastUpdateTime?.Coupons == null || OnlineLastUpdateTime.Coupons > LocalLastUpdateTime.Coupons;
            }
        }
        public bool StaffsHasUpdate
        {
            get
            {
                return OnlineLastUpdateTime?.Staffs == null || LocalLastUpdateTime?.Staffs == null || OnlineLastUpdateTime.Staffs > LocalLastUpdateTime.Staffs;
            }
        }
        public bool MatchInfosHasUpdate
        {
            get
            {
                return OnlineLastUpdateTime?.MatchInfos == null || LocalLastUpdateTime?.MatchInfos == null || OnlineLastUpdateTime.MatchInfos > LocalLastUpdateTime.MatchInfos;
            }
        }
        public bool LikeInfosHasUpdate
        {
            get
            {
                return OnlineLastUpdateTime?.LikeInfos == null || LocalLastUpdateTime?.LikeInfos == null || OnlineLastUpdateTime.LikeInfos > LocalLastUpdateTime.LikeInfos;
            }
        }
        public bool BrandsHasUpdate
        {
            get
            {
                return OnlineLastUpdateTime?.Brands == null || LocalLastUpdateTime?.Brands == null || OnlineLastUpdateTime.Brands > LocalLastUpdateTime.Brands;
            }
        }

        public void AdsUpdated()
        {
            if (OnlineLastUpdateTime != null)
            {
                LocalLastUpdateTime.Ads = OnlineLastUpdateTime.Ads;
            }
        }
        public void AppsUpdated()
        {
            if (OnlineLastUpdateTime != null)
            {
                LocalLastUpdateTime.Apps = OnlineLastUpdateTime.Apps;
            }
        }
        public void ProductCategoriesUpdated()
        {
            if (OnlineLastUpdateTime != null)
            {
                LocalLastUpdateTime.ProductCategories = OnlineLastUpdateTime.ProductCategories;
            }
        }
        public void ProductsUpdated()
        {
            if (OnlineLastUpdateTime != null)
            {
                LocalLastUpdateTime.Products = OnlineLastUpdateTime.Products;
            }
        }
        public void SkusUpdated()
        {
            if (OnlineLastUpdateTime != null)
            {
                LocalLastUpdateTime.Skus = OnlineLastUpdateTime.Skus;
            }
        }
        public void TagsUpdated()
        {
            if (OnlineLastUpdateTime != null)
            {
                LocalLastUpdateTime.Tags = OnlineLastUpdateTime.Tags;
            }
        }
        public void ProductCommentsUpdated()
        {
            if (OnlineLastUpdateTime != null)
            {
                LocalLastUpdateTime.ProductComments = OnlineLastUpdateTime.ProductComments;
            }
        }

        public void ProductsSkuPropertyValuesUpdated()
        {
            ProductsUpdated();
            SkusUpdated();
            PropertyValuesUpdated();
        }

        public void PropertiesUpdated()
        {
            if (OnlineLastUpdateTime != null)
            {
                LocalLastUpdateTime.Properties = OnlineLastUpdateTime.Properties;
            }
        }
        public void PropertyValuesUpdated()
        {
            if (OnlineLastUpdateTime != null)
            {
                LocalLastUpdateTime.PropertyValues = OnlineLastUpdateTime.PropertyValues;
            }
        }
        public void CouponsUpdated()
        {
            if (OnlineLastUpdateTime != null)
            {
                LocalLastUpdateTime.Coupons = OnlineLastUpdateTime.Coupons;
            }
        }
        public void StaffsUpdated()
        {
            if (OnlineLastUpdateTime != null)
            {
                LocalLastUpdateTime.Staffs = OnlineLastUpdateTime.Staffs;
            }
        }
        public void MatchInfosUpdated()
        {
            if (OnlineLastUpdateTime != null)
            {
                LocalLastUpdateTime.MatchInfos = OnlineLastUpdateTime.MatchInfos;
            }
        }
        public void LikeInfosUpdated()
        {
            if (OnlineLastUpdateTime != null)
            {
                LocalLastUpdateTime.LikeInfos = OnlineLastUpdateTime.LikeInfos;
            }
        }
        public void BrandsUpdated()
        {
            if (OnlineLastUpdateTime != null)
            {
                LocalLastUpdateTime.Brands = OnlineLastUpdateTime.Brands;
            }
        }
    }
}
