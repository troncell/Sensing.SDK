using Sensing.SDK.Contract;
using Sensing.SDK.Contract.Faces;
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
        private const string PostBehaviorQuery = "api/services/app/SensingDevice/PostBehaviorRecords";
        private const string PostDeviceStatusQuery = "api/services/app/SensingDevice/PostDeviceStatusRecords";
        private const string PostDeviceNetworkStatusQuery = "api/services/app/SensingDevice/PostDeviceNetworkStatusRecords";
        private const string PostFaceRecordQuery = "api/services/app/SensingDevice/PostFaceRecord";
        private string DeviceHeartBeatQuery = "api/services/app/SensingDevice/DeviceHeartBeat";
        private string GetFaceRecordsQuery = "api/services/app/SensingDevice/PostFaceRecord";


    
        public async Task<bool> PostBehaviorRecordsAsync(IEnumerable<BehaviorRecord> records)
        {
            var absolutePath = $"{DeviceBigDataServiceApiHost}{PostBehaviorQuery}?{GetBasicNameValuesQueryString()}";
            try
            {
                var result = await SendRequestAsync<IEnumerable<BehaviorRecord>, AjaxResponse<bool>>(HttpMethod.Post, absolutePath, records);
                return result.Success;
            }
            catch (Exception ex)
            {
                //logger.Error("PostBehaviorRecordsAsync", ex);
                Console.WriteLine("PostBehaviorRecordsAsync:" + ex.InnerException);
            }
            return false;
        }

        public async Task<bool> PostDeviceStatusRecordAsync(IEnumerable<DeviceStatusInput> status)
        {
            var absolutePath = $"{DeviceBigDataServiceApiHost}{PostDeviceStatusQuery}?{GetBasicNameValuesQueryString()}";
            try
            {
                var result = await SendRequestAsync<IEnumerable<DeviceStatusInput>, AjaxResponse<bool>>(HttpMethod.Post, absolutePath, status);
                return result.Success;
            }
            catch (Exception ex)
            {
                //logger.Error("PostBehaviorRecordsAsync", ex);
                Console.WriteLine("PostDeviceStatusRecordAsync:" + ex.InnerException);
            }
            return false;
        }

        public async Task<bool> PostDeviceNetworkStatusRecords(IEnumerable<DeviceNetworkStatusInput> status)
        {
            var absolutePath = $"{DeviceBigDataServiceApiHost}{PostDeviceNetworkStatusQuery}?{GetBasicNameValuesQueryString()}";
            try
            {
                var result = await SendRequestAsync<IEnumerable<DeviceNetworkStatusInput>, AjaxResponse<bool>>(HttpMethod.Post, absolutePath, status);
                return result.Success;
            }
            catch (Exception ex)
            {
                //logger.Error("PostBehaviorRecordsAsync", ex);
                Console.WriteLine("PostDeviceNetworkStatusRecords:" + ex.InnerException);
            }
            return false;
        }

        public async Task<bool> PostFaceRecordAsync(FaceRecord record)
        {
            var absolutePath = $"{DeviceBigDataServiceApiHost}{PostFaceRecordQuery}?{GetBasicNameValuesQueryString()}";
            try
            {
                var result = await SendRequestAsync<FaceRecord, AjaxResponse<bool>>(HttpMethod.Post, absolutePath, record);
                return result.Success;
            }
            catch (Exception ex)
            {
                //logger.Error("PostBehaviorRecordsAsync", ex);
                Console.WriteLine("PostFaceRecordAsync:" + ex.InnerException);
            }
            return false;
        }



        public async Task<PagedResultDto<FaceRecordOutput>> GetFaceRecordsAsync(FaceRecordInput input)
        {
            var absolutePath = $"{DeviceBigDataServiceApiHost}{GetFaceRecordsQuery}?{GetBasicNameValuesQueryString()}&sorting={input.Sorting}&maxResultCount={input.MaxResultCount}&skipCount={input.SkipCount}&collectionStartTime={input.CollectionStartTime}&collectionEndTime={input.CollectionEndTime}";
            try
            {
                var result = await SendRequestAsync<string, AjaxResponse<PagedResultDto<FaceRecordOutput>>>(HttpMethod.Get, absolutePath, null);
                return result.Result;
            }
            catch (Exception ex)
            {
                //logger.Error("PostBehaviorRecordsAsync", ex);
                Console.WriteLine("GetFaceRecordsAsync:" + ex.InnerException);
            }
            return null;
        }

    }
}
