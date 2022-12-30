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

namespace SensingHub.Sdk
{
    /// <summary>
    /// SensingHub's SDK for Casting events and web api.
    /// </summary>
    public partial class SensingHubClient
    {

        /// <summary>
        /// Receiving the remote cast events from server
        /// </summary>
        public event EventHandler<PadMessagePackageEventArgs> PadCommandArrivedEvent;

        /// <summary>
        /// Receting the cast stop event from server
        /// </summary>
        public event EventHandler<DeviceStatusPackageEventArgs> DeviceStatusChangedEvent;

        //current devices
        private static HubConnection padControlHubConnection;

        /// <summary>
        /// Connect to baseUrl for HubConnection.
        /// </summary>
        /// <param name="baseUrl"></param>
        /// <returns></returns>
        public async Task ConnectPadAsync(string baseUrl)
        {
            if (string.IsNullOrEmpty(baseUrl)) return;
            _baseUrl = baseUrl;
            if (padControlHubConnection == null)
            {
                padControlHubConnection = new HubConnectionBuilder()
                        .WithUrl($"{_baseUrl}/Connections/PadControl")
                        .Build();
                //padControlHubConnection.On<string, string>("OnRemoteCast", OnRemoteCast);
                //padControlHubConnection.On<string>("OnStopCast", OnStopCast);
                //padControlHubConnection.Closed += HubConnection_Closed;

                padControlHubConnection.On<JsonElement>("received", (data) => OnCommandReceived(data));
                await ConnectWithRetryAsync(padControlHubConnection, s_token);
                padControlHubConnection.Closed += PadControlHubConnection_Closed; ;
            }
        }

        private async Task PadControlHubConnection_Closed(Exception arg)
        {
            await Task.Delay(new Random().Next(0, 5) * 1000);
            await ConnectWithRetryAsync(castHubConnection, s_token);
        }

        private void OnCommandReceived(JsonElement data)
        {
            string packageType = data.GetProperty("Type").GetString();
            switch (packageType)
            {
                case MessagePackageTypeConstants.Notification:
                    break;
                case MessagePackageTypeConstants.Command:
                    var package = JsonConvert.DeserializeObject<PadMessagePackage>(data.GetRawText());
                    PadCommandArrivedEvent?.Invoke(this, new PadMessagePackageEventArgs(package));
                    break;
                case MessagePackageTypeConstants.DeviceStatus:
                    var statusPackage = JsonConvert.DeserializeObject<DeviceStatusPackage>(data.GetRawText());
                    DeviceStatusChangedEvent?.Invoke(this, new DeviceStatusPackageEventArgs(statusPackage));
                    break;
            }
        }
    }
}
