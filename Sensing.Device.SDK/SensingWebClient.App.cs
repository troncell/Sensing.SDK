using Sensing.SDK.Contract;
using SensingStoreCloud.Devices.Dto.SensingDevice;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Sensing.SDK
{
    partial class SensingWebClient
    {
        /// <summary>
        /// Get all the things.
        /// </summary>
        private const string GetAppsQuery = "api/services/app/SensingDevice/GetApps";

        public async Task<PagedResultDto<DeviceSoftwareSdkModel>> GetApps(int skipCount = 0, int maxCount = 300)
        {
            var absolutePath = $"{AdsServiceApiHost}{GetAppsQuery}?{GetBasicNameValuesQueryString()}&{MaxResultCount}={maxCount}&{SkipCount}={skipCount}";
            try
            {
                var webResult = await SendRequestAsync<string, AjaxResponse<PagedResultDto<DeviceSoftwareSdkModel>>>(HttpMethod.Get, absolutePath,null);
                if(webResult.Success)
                {
                    return webResult.Result;
                }
                Console.WriteLine("GetApps:" + webResult.Error.Message);
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine("GetApps:" + ex.InnerException);
            }
            return null;
        }
    }
}
