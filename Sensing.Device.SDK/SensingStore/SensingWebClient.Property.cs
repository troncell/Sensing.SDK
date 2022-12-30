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
        private const string PropertyBaseUrl = "SensingDevice";
        private const string GetPropertiesQuery = PropertyBaseUrl + "/GetProperties";

        public async Task<PagedResultDto<PropertyViewModel>> GetProperties(int skipCount = 0,int maxCount=300)
        {
            var absolutePath = $"{MainServiceApiHost}/{GetPropertiesQuery}?{GetBasicNameValuesQueryString()}&{MaxResultCount}={maxCount}&{SkipCount}={skipCount}";
            try
            {
                var webResult = await SendRequestAsync<string, AjaxResponse<PagedResultDto<PropertyViewModel>>>(HttpMethod.Get, absolutePath, null);
                return webResult.Result;
            }
            catch (Exception ex)
            {
                //logger.Error("PostBehaviorRecordsAsync", ex);
                Console.WriteLine("GetProperties:" + ex.InnerException);
            }
            return null;
        }
    }
}
