using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Specialized;
using log4net;
using System.Reflection;
using Sensing.SDK.Contract;
using System.Configuration;

namespace Sensing.SDK
{
    public partial class SensingWebClient
    {
        //public const string ServerBase = "http://game.troncell.com/";
        //public const string ServerBase = "http://wx.troncell.com/";
        //public const string ServerBase = "http://localhost:33227/";
        //public const string ServerBase = "http://139.224.15.28:142/";
        //public const string ServerBase = "http://behavior.troncell.com/";
        public readonly static string ServerBase = "http://139.224.15.28:881/";
        public readonly static string MainServiceRelativePath = "s/";
        public readonly static string ExtenalServiceRelativePath = "e/";
        public readonly static string SignalRPath = ServerBase + "signalr";
        public readonly static string Api_Relative_Path = "api/services/app/";
        public readonly static string AdsServiceApiHost = "https://ads.api.troncell.com/";
        public readonly static string DeviceBigDataServiceApiHost = "https://devicebigdata.api.troncell.com/";
        public readonly static string DeviceCenterApiHost = "https://devicecenter.api.troncell.com/";
        public readonly static string SApiHost = "https://identity.api.troncell.com/";
        public readonly static string ProductApiHost = "https://product.api.troncell.com/";
        public readonly static string RecommendApiHost = "https://recommend.api.troncell.com/";


        public readonly static string MainServiceApiHost = ServerBase + MainServiceRelativePath + Api_Relative_Path;
        public readonly static string MainServiceHost = ServerBase + MainServiceRelativePath;
        public readonly static string ExtenalServiceHost = ServerBase + ExtenalServiceRelativePath + Api_Relative_Path;


        #region Inner Keys.
        /// <summary>
        /// The subscription key.
        /// </summary>
        private string _subKey;
        private string _softwareNo;
        private string _clientNo;
        private string _deviceActivityGameSecurityKey;
        #endregion

        private static readonly ILog logger = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// The json header
        /// </summary>
        private const string JsonHeader = "application/json";

        /// <summary>
        /// The default resolver.
        /// </summary>
        private static CamelCasePropertyNamesContractResolver s_defaultResolver = new CamelCasePropertyNamesContractResolver();

        private static JsonSerializerSettings s_settings = new JsonSerializerSettings()
        {
            DateFormatHandling = DateFormatHandling.IsoDateFormat,
            DateFormatString = "yyyy-MM-dd HH:mm:ss",
            NullValueHandling = NullValueHandling.Ignore,
            ContractResolver = s_defaultResolver
        };



        private static HttpClient s_httpClient = new HttpClient();

        static SensingWebClient()
        {
            s_httpClient.Timeout = TimeSpan.FromSeconds(60);
            var appConfig = ConfigurationManager.OpenExeConfiguration(Assembly.GetExecutingAssembly().Location);
            var url = appConfig.AppSettings?.Settings["CloudServerUrl"]?.Value;
            if(!string.IsNullOrEmpty(url))
            {
                ServerBase = url;
                MainServiceApiHost = ServerBase + MainServiceRelativePath + Api_Relative_Path;
                MainServiceHost = ServerBase + MainServiceRelativePath;
               // ExtenalServiceHost = ServerBase + ExtenalServiceRelativePath + Api_Relative_Path;
               ExtenalServiceHost = "https://e.api.troncell.com/" + Api_Relative_Path;

            }
        }

        public SensingWebClient(string subscriptionKey, string softwareCode,string mac,string deviceActivityGameSecurityKey = "")
        {
            this._subKey = subscriptionKey;
            this._softwareNo = softwareCode;
            this._clientNo = mac;
            _deviceActivityGameSecurityKey = deviceActivityGameSecurityKey;
        }

        #region the json client
        private async Task<TResponse> SendRequestAsync<TRequest, TResponse>(HttpMethod httpMethod, string requestUrl, TRequest requestBody)
        {
            var request = new HttpRequestMessage(httpMethod, MainServiceApiHost);
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
            return $"softwareNo={_softwareNo}&clientNo={_clientNo}&subKey={_subKey}&securityKey={_deviceActivityGameSecurityKey}";
        }

        private void AddBasicNameValues(NameValueCollection collections)
        {
            collections.Add("softwareNo", _softwareNo);
            collections.Add("clientNo", _clientNo);
            collections.Add("subKey", _subKey);
            collections.Add("SecurityKey", _deviceActivityGameSecurityKey);
        }
        #endregion
    }
}
