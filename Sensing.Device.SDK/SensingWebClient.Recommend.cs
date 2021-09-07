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
        /// Post device data record.
        /// </summary>
        private const string GetDateMetaPhysicsQuery = "api/services/app/SensingDevice/GetDateMetaphysicsBySubkey";
        private const string GetMetaphysicsListQuery = "api/services/app/SensingDevice/GetMetaphysicsListBySubkey";
        private const string GeTagRecommendsQuery = "api​/services​/app​/SensingDevice​/GetTagRecommendsBySubkey";
        private const string GeTagRecommendsByFaceQuery = "api/services/app/SensingDevice/GetTagRecommendsByFace";

    

        public async Task<PagedResultDto<DateMetaphysicsDto>> GetDateMetaPhysics(DateTime startTime, DateTime endTime,long? typeId = null, int skipCount = 0, int maxCount = 300)
        {
            var typestring = typeId.HasValue ? typeId.Value.ToString() : "";
            var absolutePath = $"{RecommendApiHost}{GetDateMetaPhysicsQuery}?{GetBasicNameValuesQueryString()}&typeId={typestring}&startTime={startTime}&endTime={endTime}&{MaxResultCount}={maxCount}&{SkipCount}={skipCount}";
            try
            {
                var result = await SendRequestAsync<string, AjaxResponse<PagedResultDto<DateMetaphysicsDto>>>(HttpMethod.Get, absolutePath, null);
                return result.Result;
            }
            catch (Exception ex)
            {
                //logger.Error("PostBehaviorRecordsAsync", ex);
                Console.WriteLine("PostBehaviorRecordsAsync:" + ex.InnerException);
            }
            return null;
        }

        public async Task<PagedResultDto<MetaPhysicsDto>> GetMetaphysicsList(long? typeId = null, int skipCount = 0, int maxCount = 300)
        {
            var typestring = typeId.HasValue ? typeId.Value.ToString() : "";
            var absolutePath = $"{RecommendApiHost}{GetMetaphysicsListQuery}?{GetBasicNameValuesQueryString()}&typeId={typestring}&{MaxResultCount}={maxCount}&{SkipCount}={skipCount}";
            try
            {
                var result = await SendRequestAsync<string, AjaxResponse<PagedResultDto<MetaPhysicsDto>>>(HttpMethod.Get, absolutePath, null);
                return result.Result;
            }
            catch (Exception ex)
            {
                //logger.Error("PostBehaviorRecordsAsync", ex);
                Console.WriteLine("GetMetaphysicsList:" + ex.InnerException);
            }
            return null;
        }


        public async Task<FaceTagsRecommendsDto> GetTagRecommends(GetTagAndRecommendsBySubKeyInput input)
        {
            input.SubKey = _subKey;
            var absolutePath = $"{RecommendApiHost}{GeTagRecommendsQuery}?{GetBasicNameValuesQueryString()}&gender={input.Gender}&age={input.Age}&happiness={input.Happiness}&beautyScore={input.BeautyScore}";
            try
            {
                var result = await SendRequestAsync<string, AjaxResponse<FaceTagsRecommendsDto>> (HttpMethod.Get, absolutePath, null);
                return result.Result;
            }
            catch (Exception ex)
            {
                //logger.Error("PostBehaviorRecordsAsync", ex);
                Console.WriteLine("GetTagRecommends:" + ex.InnerException);
            }
            return null;
        }

        public async Task<FaceTagsRecommendsDto> GetTagRecommendsByFace(GetTagRecommendsByFaceInput input)
        {
            input.SubKey = _subKey;
            var absolutePath = $"{RecommendApiHost}{GeTagRecommendsByFaceQuery}";
            try
            {
                var result = await SendRequestAsync<GetTagRecommendsByFaceInput, AjaxResponse<FaceTagsRecommendsDto>>(HttpMethod.Post, absolutePath, input);
                return result.Result;
            }
            catch (Exception ex)
            {
                //logger.Error("PostBehaviorRecordsAsync", ex);
                Console.WriteLine("GetTagRecommendsByFace:" + ex.InnerException);
            }
            return null;
        }

    }
}
