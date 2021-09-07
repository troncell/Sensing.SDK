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
        private const string GetProductsQuery = "api/services/app/SensingDevice/GetProducts";

        private const string GetTCategoriesQuery = "api/services/app/SensingDevice/GetProductCategories";
        private const string GetMatchesQuery = "api/services/app/SensingDevice/GetMatchInfos";
        private const string GetLikesQuery = "api/services/app/SensingDevice/GetLikeInfos";
        private const string GetCommentsQuery = "api/services/app/SensingDevice/GetProductComments";
        private const string GetProductTagsQuery = "api/services/app/SensingDevice/GetTags";


        private const string MaxResultCount = "MaxResultCount";
        private const string SkipCount = "SkipCount";
        private const string Query = "Filter";


        public async Task<PagedResultDto<ProductSdkModel>> GetProducts(int skipCount = 0,int maxCount=300)
        {
            var absolutePath = $"{ProductApiHost}{GetProductsQuery}?{GetBasicNameValuesQueryString()}&{MaxResultCount}={maxCount}&{SkipCount}={skipCount}";
            try
            {
                var webResult = await SendRequestAsync<string,AjaxResponse<PagedResultDto<ProductSdkModel>>>(HttpMethod.Get, absolutePath,null);
                return webResult.Result;
            }
            catch (Exception ex)
            {
                //logger.Error("PostBehaviorRecordsAsync", ex);
                Console.WriteLine("GetProducts:" + ex.InnerException);  
            }
            return null;
        }

        public async Task<PagedResultDto<ProductCategorySDKModel>> GetProductCategories(int skipCount = 0,int maxCount = 500)
        {
            var absolutePath = $"{ProductApiHost}{GetTCategoriesQuery}?{GetBasicNameValuesQueryString()}&{MaxResultCount}={maxCount}&{SkipCount}={skipCount}";
            try
            {
                var webResult = await SendRequestAsync<string, AjaxResponse<PagedResultDto<ProductCategorySDKModel>>>(HttpMethod.Get, absolutePath, null);

                return webResult.Result;
            }
            catch (Exception ex)
            {
                //logger.Error("PostBehaviorRecordsAsync", ex);
                Console.WriteLine("GetProductCategories:" + ex.InnerException);
            }
            return null;
        }


        public async Task<PagedResultDto<MatchInfoViewModel>> GetMatchInfos(int skipCount = 0, int maxCount = 1000)
        {
            var absolutePath = $"{ProductApiHost}{GetMatchesQuery}?{GetBasicNameValuesQueryString()}&{MaxResultCount}={maxCount}&{SkipCount}={skipCount}";
            try
            {
                var webResult = await SendRequestAsync<string, AjaxResponse<PagedResultDto<MatchInfoViewModel>>>(HttpMethod.Get, absolutePath, null);

                return webResult.Result;
            }
            catch (Exception ex)
            {
                //logger.Error("PostBehaviorRecordsAsync", ex);
                Console.WriteLine("GetMatchInfos:" + ex.InnerException);
            }
            return null;
        }

        public async Task<PagedResultDto<LikeInfoViewModel>> GetLikeInfos(int skipCount = 0, int maxCount = 1000)
        {
            var absolutePath = $"{ProductApiHost}{GetLikesQuery}?{GetBasicNameValuesQueryString()}&{MaxResultCount}={maxCount}&{SkipCount}={skipCount}";
            try
            {
                var webResult = await SendRequestAsync<string, AjaxResponse<PagedResultDto<LikeInfoViewModel>>>(HttpMethod.Get, absolutePath, null);

                    return webResult.Result;
                
            }
            catch (Exception ex)
            {
                Console.WriteLine("GetLikeInfos:" + ex.InnerException);
            }
            return null;
        }

        public async Task<PagedResultDto<ProductCommentModel>> GetProductComments(int skipCount = 0, int maxCount = 1000)
        {
            var absolutePath = $"{ProductApiHost}{GetCommentsQuery}?{GetBasicNameValuesQueryString()}&{MaxResultCount}={maxCount}&{SkipCount}={skipCount}";
            try
            {
                var webResult = await SendRequestAsync<string, AjaxResponse<PagedResultDto<ProductCommentModel>>>(HttpMethod.Get, absolutePath, null);
                return webResult.Result;
            }
            catch (Exception ex)
            {
                Console.WriteLine("GetProductComments:" + ex.InnerException);
            }
            return null;
        }

        public async Task<PagedResultDto<TagSdkModel>> GetProductTags(int skipCount = 0, int maxCount = 300)
        {
            var absolutePath = $"{ProductApiHost}{GetProductTagsQuery}?{GetBasicNameValuesQueryString()}&{MaxResultCount}={maxCount}&{SkipCount}={skipCount}";
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


    }
}
