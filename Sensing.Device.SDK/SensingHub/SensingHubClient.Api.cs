using log4net;
using Microsoft.AspNetCore.SignalR.Client;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.Text.Json;
using Sensing.Device.SDK.SensingHub;
using SensingStoreCloud.Activity;
using Sensing.SDK.Contract;
using Sensing.SDK;
using System.Collections.Specialized;
using System.IO;
using System.Net.Http.Headers;

namespace SensingHub.Sdk
{
    /// <summary>
    /// SensingHub's SDK for Casting events and web api.
    /// </summary>
    public partial class SensingHubClient
    {
        private const string GetDeviceBySubKeyQuery = "api/services/app/SensingDevice/GetDeviceBySubKey";

        private const string CheckSoftwareVersion = "/software/checkupdate";
        private string _subKey = string.Empty;


        /// <summary>
        /// 获取设备下的所有资源
        /// </summary>
        /// <returns></returns>
        public async Task<DeviceInfo> GetDeviceResourcesAsync()
        {
            var absolutePath = $"{_baseUrl}/{GetDeviceBySubKeyQuery}?{GetBasicNameValuesQueryString()}";

            try
            {
                var webResult = await SendRequestAsync<string, AjaxResponse<DeviceInfo>>(HttpMethod.Get, absolutePath,null);
                return webResult.Result;
            }
            catch (Exception ex)
            {
                Console.WriteLine("GetDeviceResourcesAsync:" + ex.InnerException);
            }
            return null;
        }

        public async Task<string> GetSoftwareVersion()
        {
            var absolutePath = $"{_baseUrl}/{GetDeviceBySubKeyQuery}?{GetBasicNameValuesQueryString()}";

            try
            {
                var version = await s_httpClient.GetStringAsync(absolutePath);
                return version;
            }
            catch (Exception ex)
            {
                Console.WriteLine("GetSoftwareVersion:" + ex.InnerException);
            }
            return null;
        }



        #region the json client
        private async Task<TResponse> SendRequestAsync<TRequest, TResponse>(HttpMethod httpMethod, string requestUrl, TRequest requestBody)
        {
            var request = new HttpRequestMessage(httpMethod, _baseUrl);
            request.RequestUri = new Uri(requestUrl);
            if (requestBody != null)
            {
                if (requestBody is Stream)
                {
                    request.Content = new StreamContent(requestBody as Stream);
                    request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                }
                else if (requestBody is string)
                {
                    request.Content = new StringContent(requestBody as string);
                    request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");
                }
                else
                {
                    request.Content = new StringContent(JsonConvert.SerializeObject(requestBody, s_settings), Encoding.UTF8, JsonHeader);
                    request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                }
            }
            //else if(requestBody != null && httpMethod == HttpMethod.Post)
            //{

            //}

            HttpResponseMessage response = await s_httpClient.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                string responseContent = null;
                if (response.Content != null)
                {
                    responseContent = await response.Content.ReadAsStringAsync();
                }

                if (!string.IsNullOrWhiteSpace(responseContent))
                {
                    return JsonConvert.DeserializeObject<TResponse>(responseContent, s_settings);
                }

                return default(TResponse);
            }
            else
            {
                if (response.Content != null && response.Content.Headers.ContentType.MediaType.Contains(JsonHeader))
                {
                    var errorObjectString = await response.Content.ReadAsStringAsync();
                    ErrorAjaxResponse errorCollection = JsonConvert.DeserializeObject<ErrorAjaxResponse>(errorObjectString);
                    //return errorCollection;
                    if (errorCollection != null)
                    {
                        throw new ClientException(errorCollection.Error?.Message, response.StatusCode);
                    }
                }

                response.EnsureSuccessStatusCode();
            }

            return default(TResponse);
        }

        private async Task<TResponse> SendMultipartFormRequestAsync<TResponse>(string requestUrl, string[] files, string[] names, NameValueCollection data)
        {

            using (MultipartFormDataContent form = new MultipartFormDataContent(("Upload----" + DateTime.Now.Ticks.ToString())))
            {
                //1.1 key/value
                foreach (string key in data.Keys)
                {
                    //Content-Disposition : form-data; name="json".
                    var value = data[key];
                    if (!string.IsNullOrEmpty(value))
                    {
                        var stringContent = new StringContent(value);
                        stringContent.Headers.Add("Content-Disposition", $"form-data; name={key}");
                        form.Add(stringContent, key);
                    }
                }

                //1.2 file
                for (int index = 0; index < files.Length; index++)
                {
                    var filePath = files[index];
                    FileStream stream = File.OpenRead(filePath);
                    var streamContent = new StreamContent(stream);
                    streamContent.Headers.Add("Content-Type", "application/octet-stream");
                    streamContent.Headers.Add("Content-Disposition", $"form-data; name={names[index]}; filename={Path.GetFileName(filePath)}");
                    form.Add(streamContent, names[index], Path.GetFileName(filePath));
                }

                HttpResponseMessage response = await s_httpClient.PostAsync(requestUrl, form);

                if (response.IsSuccessStatusCode)
                {
                    string responseContent = null;
                    if (response.Content != null)
                    {
                        responseContent = await response.Content.ReadAsStringAsync();
                    }
                    if (!string.IsNullOrWhiteSpace(responseContent))
                    {
                        return JsonConvert.DeserializeObject<TResponse>(responseContent, s_settings);
                    }
                    return default(TResponse);
                }
                else
                {
                    if (response.Content != null && response.Content.Headers.ContentType.MediaType.Contains(JsonHeader))
                    {
                        var errorObjectString = await response.Content.ReadAsStringAsync();
                        Console.WriteLine(errorObjectString);
                        ClientError errorCollection = JsonConvert.DeserializeObject<ClientError>(errorObjectString);
                        if (errorCollection != null)
                        {
                            throw new ClientException(errorCollection, response.StatusCode);
                        }
                    }

                    response.EnsureSuccessStatusCode();
                }
                return default(TResponse);
            }
        }


        private string GetBasicNameValuesQueryString()
        {
            return $"subKey={_subKey}";
        }

        private void AddBasicNameValues(NameValueCollection collections)
        {
            //collections.Add("softwareNo", _softwareNo);
            //collections.Add("clientNo", _clientNo);
            collections.Add("subKey", _subKey);
            //collections.Add("SecurityKey", _deviceActivityGameSecurityKey);
        }
        #endregion
    }
}
