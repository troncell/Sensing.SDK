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
        /// <summary>
        /// Receiving the remote cast events from server
        /// </summary>
        public event EventHandler<RemoteCastEventArgs> RemoteCastEvent;

        /// <summary>
        /// Receting the cast stop event from server
        /// </summary>
        public event EventHandler<StopRemoteCastEventArgs> StopRemoteCastEvent;

        //current devices
        private static HubConnection castHubConnection;

        /// <summary>
        /// Connect to baseUrl for HubConnection.
        /// </summary>
        /// <param name="baseUrl"></param>
        /// <returns></returns>
        public async Task ConnectAsync(string baseUrl)
        {

            if (string.IsNullOrEmpty(baseUrl)) return; 
            _baseUrl = baseUrl;
            if (castHubConnection == null)
            {
                castHubConnection = new HubConnectionBuilder()
                        .WithUrl($"{_baseUrl}/remotecast")
                        .Build();
                castHubConnection.On<string, string>("OnRemoteCast", OnRemoteCast);
                castHubConnection.On<string>("OnStopCast", OnStopCast);
                castHubConnection.Closed += HubConnection_Closed;
                await ConnectWithRetryAsync(castHubConnection, s_token);
            }
        }

        private async Task HubConnection_Closed(Exception arg)
        {
            await Task.Delay(new Random().Next(0, 5) * 1000);
            await ConnectWithRetryAsync(castHubConnection, s_token);
        }

        /// <summary>
        /// Send Cast Event to Server, and dispatch this to all clients
        /// </summary>
        /// <param name="fileName">casting file</param>
        /// <param name="deviceId">target device</param>
        public void RemoteCast(string fileName, string deviceId)
        {
            if(castHubConnection.State == HubConnectionState.Connected)
            {
                try
                {
                    castHubConnection.SendAsync("RemoteCast", fileName, deviceId);
                }
                catch(Exception ex)
                {
                    logger.Error("Send RemoteCast Event Exception", ex);
                }
            }
        }

        /// <summary>
        /// Login target device for controlling, currently, just one controller can only control one device.
        /// </summary>
        /// <param name="deviceId">target device for loging in</param>
        /// <returns></returns>
        public async Task<bool> Login(string deviceId)
        {
            if (castHubConnection.State == HubConnectionState.Connected)
            {
               await castHubConnection.SendAsync("Login", deviceId);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Stop casting
        /// </summary>
        /// <param name="deviceId">target device</param>
        public void StopCast(string deviceId)
        {
            if (castHubConnection.State == HubConnectionState.Connected)
            {
                castHubConnection.SendAsync("StopCast", deviceId);
            }
        }

        private Task OnRemoteCast(string fileName, string deviceId)
        {
            Debug.WriteLine($"OnRemoteCast {fileName} {deviceId}");
            RemoteCastEvent?.Invoke(this,new RemoteCastEventArgs(fileName,deviceId));
            return Task.CompletedTask;
        }

        private Task OnStopCast(string deviceId)
        {
            Debug.WriteLine($"Stop Cast {deviceId}");
            StopRemoteCastEvent?.Invoke(this, new StopRemoteCastEventArgs(deviceId));
            return Task.CompletedTask;
        }
    }
}
