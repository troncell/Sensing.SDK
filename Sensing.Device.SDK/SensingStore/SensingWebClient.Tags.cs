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
        //private const string TagBaseUrl = "SensingDevice";
        //private const string GetTagsQuery = TagBaseUrl + "/GetTags";

        //public async Task<PagedResultDto<TagSdkModel>> GetTags(int skipCount = 0,int maxCount=300)
        //{
        //    var absolutePath = $"{MainServiceApiHost}/{GetTagsQuery}?{GetBasicNameValuesQueryString()}&{MaxResultCount}={maxCount}&{SkipCount}={skipCount}";
        //    try
        //    {
        //        var webResult = await SendRequestAsync<string, AjaxResponse<PagedResultDto<TagSdkModel>>>(HttpMethod.Get, absolutePath, null);
        //        return webResult.Result;

        //    }
        //    catch (Exception ex)
        //    {
        //        //logger.Error("PostBehaviorRecordsAsync", ex);
        //        Console.WriteLine("GetTags:" + ex.InnerException);
        //    }
        //    return null;
        //}
    }
}
