using Sensing.SDK.Contract;
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
        private const string CouponsBaseUrl = "SensingDevice";

        private const string GetCouponsQuery = CouponsBaseUrl + "/GetCoupons";

        public async Task<PagedResultDto<CouponViewModel>> GetCoupons(int skipCount = 0,int maxCount=300)
        {
            var absolutePath = $"{MainServiceApiHost}/{GetCouponsQuery}?{GetBasicNameValuesQueryString()}&{MaxResultCount}={maxCount}&{SkipCount}={skipCount}";
            try
            {
                var webResult = await SendRequestAsync<string, AjaxResponse<PagedResultDto<CouponViewModel>>>(HttpMethod.Get, absolutePath, null);
                return webResult.Result;
            }
            catch (Exception ex)
            {
                //logger.Error("PostBehaviorRecordsAsync", ex);
                Console.WriteLine("GetCoupons:" + ex.InnerException);
            }
            return null;
        }
    }
}
