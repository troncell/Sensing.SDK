using Sensing.SDK.Contract;
using SensingStoreCloud.Devices.Dto.SensingDevice;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Sensing.Device.SDK.Dto;

namespace Sensing.SDK
{
    partial class SensingWebClient
    {
        /// <summary>
        /// Get all the things.
        /// </summary>
        
        private const string GetAdsQuery = "api/services/app/SensingDevice/GetAds";
        private const string GetAdAndAppTimelinesInAWeekQuery = "api/services/app/SensingDevice/GetAdAndAppTimelinesInAWeek";
        private const string GetAdsTagsQuery = "api/services/app/SensingDevice/GetTags";



        public async Task<PagedResultDto<AdsSdkModel>> GetAds(int skipCount = 0,int maxCount=300)
        {
            var absolutePath = $"{AdsServiceApiHost}{GetAdsQuery}?{GetBasicNameValuesQueryString()}&{MaxResultCount}={maxCount}&{SkipCount}={skipCount}";
            try
            {
                var webResult = await SendRequestAsync<string, AjaxResponse<PagedResultDto<AdsSdkModel>>>(HttpMethod.Get, absolutePath,null);
                if(webResult.Success)
                {
                    return webResult.Result;
                }
                Console.WriteLine("GetAds:" + webResult.Error.Message);
                return null;
            }
            catch (Exception ex)
            {
                //logger.Error("PostBehaviorRecordsAsync", ex);
                Console.WriteLine("GetAds:" + ex.InnerException);
            }
            return null;
        }


        public async Task<List<AdAndAppTimelineScheduleViewModel>> GetAdAndAppTimelinesInAWeek(DateTime startTime)
        {
            var absolutePath = $"{AdsServiceApiHost}{GetAdAndAppTimelinesInAWeekQuery}?{GetBasicNameValuesQueryString()}&StartTime={startTime}";
            try
            {
                var webResult = await SendRequestAsync<string, AjaxResponse<List<AdAndAppTimelineScheduleViewModel>>>(HttpMethod.Get, absolutePath, null);
                if (webResult.Success)
                {
                    return webResult.Result;
                }
                Console.WriteLine("GetAdAndAppTimelinesInAWeek:" + webResult.Error.Message);
                return null;
            }
            catch (Exception ex)
            {
                //logger.Error("PostBehaviorRecordsAsync", ex);
                Console.WriteLine("GetAdAndAppTimelinesInAWeek:" + ex.InnerException);
            }
            return null;
        }

        public async Task<PagedResultDto<TagSdkModel>> GetAdsTags(int skipCount = 0, int maxCount = 300)
        {
            var absolutePath = $"{AdsServiceApiHost}{GetAdsTagsQuery}?{GetBasicNameValuesQueryString()}&{MaxResultCount}={maxCount}&{SkipCount}={skipCount}";
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
        
         public async Task<List<TagSdkModel>> GetAdsTags(string subKey)
    {
        var absolutePath = $"https://ads.api.troncell.com/api/services/app/SensingDevice/GetTags?Subkey={subKey}";
        try
        {
            List<TagSdkModel> appDtos = new List<TagSdkModel>();
            int maxResultCount = 100;
            int skipCount = 0;
            int totalCount;
            using (var httpClient = new HttpClient())
            {
                do
                {
                    var syncTagsUrl = $"{absolutePath}&MaxResultCount={maxResultCount}&SkipCount={skipCount}";
                    var response = await httpClient.GetAsync(syncTagsUrl);
                    response.EnsureSuccessStatusCode();
                    var jsonString = await response.Content.ReadAsStringAsync();
                    var result =  JsonConvert.DeserializeObject<ApiDto<PagedResultDto<TagSdkModel>>>(jsonString);
                    totalCount = result.result.TotalCount;
                    appDtos.AddRange(result.result.Items);
        
                    skipCount += maxResultCount;
                } while (skipCount < totalCount);  
            }
               

            return appDtos;
        }
        catch (Exception e)
        {
            throw new Exception(e.Message);
        }
    }
    
    public async Task<List<DeviceSoftwareSdkModel>> GetApps(string subKey)
    {
        var absolutePath = $"https://ads.api.troncell.com/api/services/app/SensingDevice/GetApps?Subkey={subKey}";
        try
        {
            List<DeviceSoftwareSdkModel> appDtos = new List<DeviceSoftwareSdkModel>();
            int maxResultCount = 100;
            int skipCount = 0;
            int totalCount;
            using (var httpClient = new HttpClient())
            {
                do
                {
                    var syncTagsUrl = $"{absolutePath}&MaxResultCount={maxResultCount}&SkipCount={skipCount}";
                    var response = await httpClient.GetAsync(syncTagsUrl);
                    response.EnsureSuccessStatusCode();
                    var jsonString = await response.Content.ReadAsStringAsync();
                    var result =  JsonConvert.DeserializeObject<ApiDto<PagedResultDto<DeviceSoftwareSdkModel>>>(jsonString);
                    totalCount = result.result.TotalCount;
                    appDtos.AddRange(result.result.Items);
        
                    skipCount += maxResultCount;
                } while (skipCount < totalCount);  
            }
               

            return appDtos;
        }
        catch (Exception e)
        {
            throw new Exception(e.Message);
        }
    }
    
    
    public async Task<List<StaffSdkModel>> GetStaffs(string subKey)
    {
        var absolutePath = $"https://ads.api.troncell.com/api/services/app/SensingDevice/GetApps?Subkey={subKey}";
        try
        {
            List<StaffSdkModel> appDtos = new List<StaffSdkModel>();
            int maxResultCount = 100;
            int skipCount = 0;
            int totalCount;
            using (var httpClient = new HttpClient())
            {
                do
                {
                    var syncTagsUrl = $"{absolutePath}&MaxResultCount={maxResultCount}&SkipCount={skipCount}";
                    var response = await httpClient.GetAsync(syncTagsUrl);
                    response.EnsureSuccessStatusCode();
                    var jsonString = await response.Content.ReadAsStringAsync();
                    var result =  JsonConvert.DeserializeObject<ApiDto<PagedResultDto<StaffSdkModel>>>(jsonString);
                    totalCount = result.result.TotalCount;
                    appDtos.AddRange(result.result.Items);
        
                    skipCount += maxResultCount;
                } while (skipCount < totalCount);  
            }
               

            return appDtos;
        }
        catch (Exception e)
        {
            throw new Exception(e.Message);
        }
    }
    
    
    public async Task<List<MetaPhysicsDto>> GetMetaphysicsListBySubkey(string subKey)
    {
        var absolutePath = $"https://ads.api.troncell.com/api/services/app/SensingDevice/GetApps?Subkey={subKey}";
        try
        {
            List<MetaPhysicsDto> appDtos = new List<MetaPhysicsDto>();
            int maxResultCount = 100;
            int skipCount = 0;
            int totalCount;
            using (var httpClient = new HttpClient())
            {
                do
                {
                    var syncTagsUrl = $"{absolutePath}&MaxResultCount={maxResultCount}&SkipCount={skipCount}";
                    var response = await httpClient.GetAsync(syncTagsUrl);
                    response.EnsureSuccessStatusCode();
                    var jsonString = await response.Content.ReadAsStringAsync();
                    var result =  JsonConvert.DeserializeObject<ApiDto<PagedResultDto<MetaPhysicsDto>>>(jsonString);
                    totalCount = result.result.TotalCount;
                    appDtos.AddRange(result.result.Items);
        
                    skipCount += maxResultCount;
                } while (skipCount < totalCount);  
            }
               

            return appDtos;
        }
        catch (Exception e)
        {
            throw new Exception(e.Message);
        }
    }

    }
}
