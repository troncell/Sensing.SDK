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

namespace SensingHub.Sdk
{
    /// <summary>
    /// SensingHub's SDK for Casting events and web api.
    /// </summary>
    public partial class SensingHubClient
    {
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

        private static CancellationToken s_token = new CancellationToken();

        private async Task<bool> ConnectWithRetryAsync(HubConnection connection, CancellationToken token)
        {
            // Keep trying to until we can start or the token is canceled.
            while (true)
            {
                try
                {
                    await connection.StartAsync(token);
                    Debug.Assert(connection.State == HubConnectionState.Connected);
                    return true;
                }
                catch when (token.IsCancellationRequested)
                {
                    return false;
                }
                catch (Exception ex)
                {
                    // Failed to connect, trying again in 5000 ms.
                    Debug.Assert(connection.State == HubConnectionState.Disconnected);
                    await Task.Delay(5000);
                }
            }
        }

        private string _baseUrl = string.Empty;

    }
}
