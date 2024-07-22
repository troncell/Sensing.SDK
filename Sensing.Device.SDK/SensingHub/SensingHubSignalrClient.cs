using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using Sensing.Device.SDK.SensingHub.Constants;

namespace Sensing.Device.SDK.SensingHub
{
    public class SensingHubSignalrClient
    {
        private Microsoft.AspNetCore.SignalR.Client.HubConnection _connection;
        private static string NotifyMethod = "NotifyDataChanged";
        private static string Received = "received";
        private string _baseUrl = string.Empty;

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
        
        public async Task<bool> DeviceLogin(long deviceId, string subKey)
        {
            if (_connection.State == HubConnectionState.Connected)
            {
                await _connection.SendAsync("Login", deviceId);
                var deviceLoginInput = new 
                {
                    CommonText = new 
                    {
                        DeviceId = deviceId,
                        SubKey = subKey
                    },
                    Type = SignalrCommonType.Auth
                };
                await _connection.InvokeAsync(NotifyMethod,
                    deviceLoginInput, SignalrFromEnum.HallAreaDevice);
                return true;
            }

            return false;
        }

        public async Task<bool> PadLogin(long deviceId, string subKey)
        {
            if (_connection.State == HubConnectionState.Connected)
            {
                var input = new 
                {
                    CommonText = new 
                    {
                        DeviceId = deviceId,
                        SubKey = subKey
                    },
                    Type = SignalrCommonType.Auth
                };
                await _connection.InvokeAsync(NotifyMethod,
                    input, SignalrFromEnum.HallAreaPad);
                return true;
            }

            return false;
        }

        public async Task<bool> LockArea(long areaId)
        {
            if (_connection.State == HubConnectionState.Connected)
            {
                var input = new 
                {
                    CommonText = new 
                    {
                        HallAreaId = areaId,
                    },
                    Type = SignalrCommonType.AreaLock
                };
                await _connection.InvokeAsync(NotifyMethod,
                    input, SignalrFromEnum.HallAreaPad);
                return true;
            }
            return false;
        }

        public async Task<bool> ReleaseAreaLock(long areaId)
        {
            if (_connection.State == HubConnectionState.Connected)
            {
                var input = new 
                {
                    CommonText = new 
                    {
                        HallAreaId = areaId,
                    },
                    Type = SignalrCommonType.AreaRelease
                };
                await _connection.InvokeAsync(NotifyMethod,
                    input, SignalrFromEnum.HallAreaPad);
                return true;
            }
            return false; 
        }
        
        public async Task<bool> DeviceLock(long deviceId)
        {
            if (_connection.State == HubConnectionState.Connected)
            {
                var input = new 
                {
                    CommonText = new 
                    {
                        DeviceId = deviceId,
                    },
                    Type = SignalrCommonType.DeviceLock
                };
                await _connection.InvokeAsync(NotifyMethod,
                    input, SignalrFromEnum.HallAreaPad);
                return true;
            }
            return false; 
        }
        
        public async Task<bool> ReleaseDeviceLock(long deviceId)
        {
            if (_connection.State == HubConnectionState.Connected)
            {
                var input = new 
                {
                    CommonText = new 
                    {
                        DeviceId = deviceId,
                    },
                    Type = SignalrCommonType.DeviceRelease
                };
                await _connection.InvokeAsync(NotifyMethod,
                    input, SignalrFromEnum.HallAreaPad);
                return true;
            }
            return false; 
        }
        
        public async Task<bool> ControlArea(long areaId,long controlId)
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
                    Type = SignalrCommonType.AreaControl
                };
                await _connection.InvokeAsync(NotifyMethod,
                    input, SignalrFromEnum.HallAreaPad);
                return true;
            }
            return false; 
        }
        
        public async Task<bool> ControlDevice(long deviceId,long controlId)
        {
            if (_connection.State == HubConnectionState.Connected)
            {
                var input = new 
                {
                    CommonText = new 
                    {
                        DeviceId = deviceId,
                        ControlId = controlId,
                    },
                    Type = SignalrCommonType.DeviceControl
                };
                await _connection.InvokeAsync(NotifyMethod,
                    input, SignalrFromEnum.HallAreaPad);
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