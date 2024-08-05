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
using Sensing.Device.SDK.SensingHub.Constants;

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

        
        private Microsoft.AspNetCore.SignalR.Client.HubConnection _connection;
        private static readonly string NotifyMethod = "NotifyDataChanged";
        private static readonly string Received = "received";

        /// <summary>
        /// Connect to baseUrl for HubConnection.
        /// </summary>
        /// <param name="baseUrl"></param>
        /// <returns></returns>
        public async Task ConnectAsync(string baseUrl)
        {

            if (string.IsNullOrEmpty(baseUrl)) return; 
            _baseUrl = baseUrl;
            if (_connection == null)
            {
                _connection = new HubConnectionBuilder()
                        .WithUrl($"{_baseUrl}/remotecast")
                        .Build();
                _connection.On<string, string>("OnRemoteCast", OnRemoteCast);
                _connection.On<string>("OnStopCast", OnStopCast);
                _connection.Closed += HubConnection_Closed;
                await ConnectWithRetryAsync(_connection, s_token);
            }
        }

        private async Task HubConnection_Closed(Exception arg)
        {
            await Task.Delay(new Random().Next(0, 5) * 1000);
            await ConnectWithRetryAsync(_connection, s_token);
        }

        /// <summary>
        /// Send Cast Event to Server, and dispatch this to all clients
        /// </summary>
        /// <param name="fileName">casting file</param>
        /// <param name="deviceId">target device</param>
        public void RemoteCast(string fileName, string deviceId)
        {
            if(_connection.State == HubConnectionState.Connected)
            {
                try
                {
                    _connection.SendAsync("RemoteCast", fileName, deviceId);
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
            if (_connection.State == HubConnectionState.Connected)
            {
               await _connection.SendAsync("Login", deviceId);
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
            if (_connection.State == HubConnectionState.Connected)
            {
                _connection.SendAsync("StopCast", deviceId);
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
        
        public async Task PadConnect(string baseUrl)
        {
            if (baseUrl == null) return;
            _baseUrl = baseUrl;
            if (_connection == null)
            {
                _connection = new HubConnectionBuilder()
                    .WithUrl($"{_baseUrl}/LocalSensingDevice")
                    .Build();

                _connection.Closed += async (error) =>
                {
                    await Task.Delay(new Random().Next(0, 5) * 1000);
                    await _connection.StartAsync();
                }; 
            }
            try
            {
                await _connection.StartAsync();
                Console.WriteLine("Connection started");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to start connection: {ex.Message}");
            }
        }

        public async Task DeviceConnect(string baseUrl)
        {
            if (baseUrl == null) return;
            _baseUrl = baseUrl;
            if (_connection == null)
            {
                _connection = new HubConnectionBuilder()
                    .WithUrl($"{_baseUrl}/LocalSensingDevice")
                    .Build();
                _connection.Closed += async (error) =>
                {
                    await Task.Delay(new Random().Next(0, 5) * 1000);
                    await _connection.StartAsync();
                };
            }
            try
            {
                await _connection.StartAsync();
                Console.WriteLine("Connection started");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to start connection: {ex.Message}");
            }
        }
        
        public async Task<bool> DeviceLogin(string subKey)
        {
            if (_connection.State == HubConnectionState.Connected)
            {
                // await _connection.SendAsync("Login", deviceId);
                var deviceLoginInput = new 
                {
                    CommonText = new 
                    {
                        SubKey = subKey
                    },
                    Type = SignalrCommonType.Auth,
                    SubKey = subKey 

                };
                await _connection.InvokeAsync(NotifyMethod,
                    deviceLoginInput, SignalrFromEnum.HallAreaDevice);
                return true;
            }

            return false;
        }

        public async Task<bool> PadLogin(string subKey)
        {
            if (_connection.State == HubConnectionState.Connected)
            {
                var input = new 
                {
                    CommonText = new 
                    {
                        SubKey = subKey
                    },
                    Type = SignalrCommonType.Auth,
                    SubKey = subKey 
                };
                await _connection.InvokeAsync(NotifyMethod,
                    input, SignalrFromEnum.HallAreaPad);
                return true;
            }

            return false;
        }

        public async Task<bool> LockArea(string subKey, long areaId)
        {
            if (_connection.State == HubConnectionState.Connected)
            {
                var input = new 
                {
                    CommonText = new 
                    {
                        HallAreaId = areaId,
                    },
                    Type = SignalrCommonType.AreaLock,
                    SubKey = subKey
                };
                await _connection.InvokeAsync(NotifyMethod,
                    input, SignalrFromEnum.HallAreaPad);
                return true;
            }
            return false;
        }

        public async Task<bool> ReleaseAreaLock(string subKey,long areaId)
        {
            if (_connection.State == HubConnectionState.Connected)
            {
                var input = new 
                {
                    CommonText = new 
                    {
                        HallAreaId = areaId,
                    },
                    Type = SignalrCommonType.AreaRelease,
                    SubKey = subKey
                };
                await _connection.InvokeAsync(NotifyMethod,
                    input, SignalrFromEnum.HallAreaPad);
                return true;
            }
            return false; 
        }
        
        public async Task<bool> DeviceLock(string subKey)
        {
            if (_connection.State == HubConnectionState.Connected)
            {
                var input = new 
                {
                    CommonText = new 
                    {
                        SubKey = subKey,
                    },
                    Type = SignalrCommonType.DeviceLock,
                    SubKey = subKey
                };
                await _connection.InvokeAsync(NotifyMethod,
                    input, SignalrFromEnum.HallAreaPad);
                return true;
            }
            return false; 
        }
        
        public async Task<bool> ReleaseDeviceLock(string subKey)
        {
            if (_connection.State == HubConnectionState.Connected)
            {
                var input = new 
                {
                    CommonText = new 
                    {
                        SubKey = subKey,
                    },
                    Type = SignalrCommonType.DeviceRelease,
                    SubKey = subKey
                };
                await _connection.InvokeAsync(NotifyMethod,
                    input, SignalrFromEnum.HallAreaPad);
                return true;
            }
            return false; 
        }
        
        public async Task<bool> ControlArea(string subKey, long areaId,long controlId)
        {
            if (_connection.State == HubConnectionState.Connected)
            {
                var input = new 
                {
                    CommonText = new 
                    {
                        HallAreaId = areaId,
                        ControlId = controlId,
                    },
                    Type = SignalrCommonType.AreaControl,
                    SubKey = subKey
                };
                await _connection.InvokeAsync(NotifyMethod,
                    input, SignalrFromEnum.HallAreaPad);
                return true;
            }
            return false; 
        }
        
        public async Task<bool> ControlDevice(string subKey,long controlId)
        {
            if (_connection.State == HubConnectionState.Connected)
            {
                var input = new 
                {
                    CommonText = new 
                    {
                        SubKey = subKey,
                        ControlId = controlId,
                    },
                    Type = SignalrCommonType.DeviceControl,
                    SubKey = subKey
                };
                await _connection.InvokeAsync(NotifyMethod,
                    input, SignalrFromEnum.HallAreaPad);
                return true;
            }
            return false; 
        }
        
        public async Task<bool> BatchControlDevice(List<string> subKeyList,string fromSubKey,string data)
        {
            if (_connection.State == HubConnectionState.Connected)
            {
                var input = new 
                {
                    CommonText = new 
                    {
                        SubKeys = subKeyList,
                        Data = data,
                    },
                    Type = SignalrCommonType.DeviceControlBatch,
                    SubKey = fromSubKey
                };
                await _connection.InvokeAsync(NotifyMethod,
                    input, SignalrFromEnum.HallAreaDevice);
                return true;
            }
            return false; 
        }
        
        public  void OnMessageReceived<T>(Action<T> handler)
        {
            if (_connection == null)
            {
                throw new InvalidOperationException("Connection is not initialized.");
            }

            _connection.On(Received, handler);
        }
    }
}
