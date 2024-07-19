using Sensing.SDK.Contract;
using SensingStoreCloud.Devices.Dto;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

namespace Sensing.SDK
{
    partial class SensingWebClient
    {
        /// <summary>
        /// Get all the things.
        /// </summary>
        private const string RegisterDeviceQuery = "api/services/app/SensingDevice/RegisterDevice";

        private const string DeviceInfoQuery = "api/services/app/SensingDevice/GetDeviceInfo";
        private const string GetDeviceSubkeyByHardwareCodeQuery = "api/services/app/SensingDevice/GetDeviceSubkeyByHardwareCode";
        private const string GetDeviceAppPodVersionQuery = "api/services/app/SensingDevice/GetDeviceAppPodVersion";
        private const string ChangeDeviceApppodVersionQuery = "api/services/app/SensingDevice/ChangeDeviceApppodVersion";
        private const string UploadScreenShotQuery = "api/services/app/SensingDevice/UploadScreenShot";
        //private const string LoginInfoQuery = DeviceBaseUrl + "/Login";
        private const string GetBrandsQuery = "api/services/app/SensingDevice/GetBrands";





        public async Task<AjaxResponse<DeviceOutput>> RegisterDeviceAsyn(RegisterDeviceInput device)
        {
            var absolutePath = $"{DeviceCenterApiHost}{RegisterDeviceQuery}?{GetBasicNameValuesQueryString()}";
            try
            {
                var pagedList = await SendRequestAsync<RegisterDeviceInput, AjaxResponse<DeviceOutput>>(HttpMethod.Post, absolutePath, device);
                if(pagedList.Success)
                {
                    return pagedList;
                }
            }
            catch (Exception ex)
            {
                //logger.Error("PostBehaviorRecordsAsync", ex);
                Console.WriteLine("RegisterDevice:" + ex.InnerException);
            }
            return null;
        }


        public async Task<AjaxResponse<DeviceOutput>> GetDeviceInfo()
        {
            var absolutePath = $"{DeviceCenterApiHost}{DeviceInfoQuery}?{GetBasicNameValuesQueryString()}";
            try
            {
                var pagedList = await SendRequestAsync<string, AjaxResponse<DeviceOutput>>(HttpMethod.Get, absolutePath, null);
                return pagedList;
            }
            catch (Exception ex)
            {
                //logger.Error("PostBehaviorRecordsAsync", ex);
                Console.WriteLine("GetDeviceInfo:" + ex.InnerException);
            }
            return null;
        }


        //public async Task<DeviceStaffLoginResultViewModel> DeviceLogin(DeviceSdkLoginViewModel loginVM)
        //{
        //    var absolutePath = $"{MainServiceApiHost}/{LoginInfoQuery}?{GetBasicNameValuesQueryString()}";
        //    try
        //    {
        //        var loginResult = await SendRequestAsync<string, AjaxResponse<DeviceStaffLoginResultViewModel>>(HttpMethod.Post, absolutePath, null);
        //        return loginResult.Result;
        //    }
        //    catch (Exception ex)
        //    {
        //        //logger.Error("PostBehaviorRecordsAsync", ex);
        //        Console.WriteLine("DeviceLogin:" + ex.InnerException);
        //    }
        //    return null;
        //}

        public async Task<DeviceAppPodVersionModel> GetDeviceAppPodVersion()
        {
            var absolutePath = $"{DeviceCenterApiHost}{GetDeviceAppPodVersionQuery}?{GetBasicNameValuesQueryString()}";
            try
            {
                var appPodVersion = await SendRequestAsync<string, AjaxResponse<DeviceAppPodVersionModel>>(HttpMethod.Get, absolutePath, null);
                return appPodVersion.Result;
            }
            catch (Exception ex)
            {
                //logger.Error("PostBehaviorRecordsAsync", ex);
                Console.WriteLine("GetDeviceAppPodVersion:" + ex.InnerException);
            }
            return null;
        }

        public async Task<bool> ChangeDeviceApppodVersion(ChangeDeviceApppodVersionInput input)
        {
            input.Subkey = _subKey;
            var absolutePath = $"{DeviceCenterApiHost}{ChangeDeviceApppodVersionQuery}?{GetBasicNameValuesQueryString()}";
            try
            {
                var loginResult = await SendRequestAsync<ChangeDeviceApppodVersionInput, AjaxResponse<string>>(HttpMethod.Post, absolutePath, input);
                return loginResult.Success;
            }
            catch (Exception ex)
            {
                //logger.Error("PostBehaviorRecordsAsync", ex);
                Console.WriteLine("DeviceLogin:" + ex.InnerException);
            }
            return false;
        }


        public async Task<string> GetDeviceSubkeyByHardwareCode(string hardwareid)
        {
            var absolutePath = $"{DeviceCenterApiHost}{GetDeviceSubkeyByHardwareCodeQuery}?HardwareCode={hardwareid}";
            try
            {
                var pagedList = await SendRequestAsync<string, AjaxResponse<string>>(HttpMethod.Get, absolutePath, null);
                return pagedList.Result;
            }
            catch (Exception ex)
            {
                //logger.Error("PostBehaviorRecordsAsync", ex);
                Console.WriteLine("GetDeviceInfo:" + ex.InnerException);
            }
            return null;
        }


        public async Task<bool> UploadScreenShot(ScreenshotInput screenshot)
        {
           var absolutePath = $"{DeviceCenterApiHost}{UploadScreenShotQuery}?{GetBasicNameValuesQueryString()}";
            try
            {
                var loginResult = await SendRequestAsync<ScreenshotInput, AjaxResponse<string>>(HttpMethod.Post, absolutePath, screenshot);
                return loginResult.Success;
            }
            catch (Exception ex)
            {
                //logger.Error("PostBehaviorRecordsAsync", ex);
                Console.WriteLine("DeviceLogin:" + ex.InnerException);
            }
            return false;
        }

        public async Task<bool> DeviceHeartBeatAsync(DeviceHeartBeatInput device)
        {
            var absolutePath = $"{DeviceCenterApiHost}{DeviceHeartBeatQuery}?{GetBasicNameValuesQueryString()}";
            try
            {
                var result = await SendRequestAsync<DeviceHeartBeatInput, AjaxResponse<bool>>(HttpMethod.Post, absolutePath, device);
                if (result != null)
                {
                    return result.Success;
                }
                return false;
            }
            catch (Exception ex)
            {
                //logger.Error("PostBehaviorRecordsAsync", ex);
                Console.WriteLine("RegisterDevice:" + ex.InnerException);
            }
            return false;
        }

        public async Task<PagedResultDto<BrandDto>> GetBrands(int skipCount = 0, int maxCount = 100)
        {
            var absolutePath = $"{DeviceCenterApiHost}{GetBrandsQuery}?{GetBasicNameValuesQueryString()}&{MaxResultCount}={maxCount}&{SkipCount}={skipCount}";
            try
            {
                var webResult = await SendRequestAsync<string, AjaxResponse<PagedResultDto<BrandDto>>>(HttpMethod.Get, absolutePath, null);
                return webResult.Result;
            }
            catch (Exception ex)
            {
                Console.WriteLine("GetBrands:" + ex.InnerException);
            }
            return null;
        }



        public async Task<PagedResultDto<TagSdkModel>> GetDeviceTags(int skipCount = 0, int maxCount = 300)
        {
            var absolutePath = $"{DeviceCenterApiHost}{GetAdsTagsQuery}?{GetBasicNameValuesQueryString()}&{MaxResultCount}={maxCount}&{SkipCount}={skipCount}";
            try
            {
                var webResult = await SendRequestAsync<string, AjaxResponse<PagedResultDto<TagSdkModel>>>(HttpMethod.Get, absolutePath, null);
                return webResult.Result;
            }
            catch (Exception ex)
            {
                //logger.Error("PostBehaviorRecordsAsync", ex);
                Console.WriteLine("GetTags:" + ex.InnerException);
            }
            return null;
        }

        public async Task<CartQrcodeOutput> AddGoodsToCar(List<ShoppingCartItem> shoppingCart)
        {
            var absolutePath = $"https://sync.api.troncell.com/api/services/app/ThingSync/AddGoodsToCar?{GetBasicNameValuesQueryString()}";
            try
            {
                ShoppingCartInput input = new ShoppingCartInput
                {
                    Longterm = true,
                    Item_ids = String.Join(",",shoppingCart.Select(s => $"{s.ItemId}_{s.SkuId}_{s.ItemCount}"))
                };
                var loginResult = await SendRequestAsync<ShoppingCartInput, AjaxResponse<CartQrcodeOutput>>(HttpMethod.Post, absolutePath, input);
                return loginResult.Result;
            }
            catch (Exception ex)
            {
                //logger.Error("PostBehaviorRecordsAsync", ex);
                Console.WriteLine("DeviceLogin:" + ex.InnerException);
            }
            return null;
        }


    }
}
