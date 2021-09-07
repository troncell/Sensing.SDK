using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Connecting;
using MQTTnet.Client.Disconnecting;
using MQTTnet.Client.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Sensing.Mqtt
{
    public class ActionReceivedMessageEventArgs : EventArgs
    {
        public JObject Message { get; set; }
    }

    public class DeviceStatusEvent
    {
        public string Action { get; set; } = "sensing-status";
        public string Status { get; set; } = "offline";
        public string DeviceId { get; set; }
    }

    public delegate void ActionReceivedEventHandler(object sender, ActionReceivedMessageEventArgs e);
    public class SensingStoreMqttClient
    {
        public const string DeviceControllerTopic = "sensing.device.controller";
        public const string DeviceLogStatusTopic = "sensing.device.log.status";
        public static string RabbitMQAddress = "139.224.23.171";
        public static string UserName = "troncell";
        public static string Password = "1qazTronCell@WSX";
        public DeviceStatusEvent _deviceStatusEvent = new DeviceStatusEvent();

        public event ActionReceivedEventHandler ActionMessageReceived;

        private IMqttClient _client;
        private readonly string _deviceId;
        private readonly string _tenantId;
        private readonly string _guid;
        private readonly string _myTopic;
        IMqttClientOptions _options;
        public SensingStoreMqttClient(string tenantId, string clientId)
        {
            _deviceId = clientId;
            _tenantId = tenantId;
            _guid = Guid.NewGuid().ToString("N").Substring(0, 8);
            _myTopic = $"{DeviceControllerTopic}.tenant-{_tenantId}.device-{_deviceId}";
            var uniqueId = $"tenant:{_tenantId}-device:{_deviceId}-{_guid}-";

            _deviceStatusEvent.DeviceId = _deviceId;

            //设置遗嘱消息，当设备离线时间大于1.5倍的心跳时间,会发布此消息到特定topic.
            var willMessageBuilder = new MqttApplicationMessageBuilder();
            var willMsg = willMessageBuilder
                .WithTopic(DeviceLogStatusTopic)
                .WithPayload(JsonConvert.SerializeObject(_deviceStatusEvent))
                .WithRetainFlag(true)
                .Build();

            var factory = new MqttFactory();

            // Create TCP based options using the builder.
            _options = new MqttClientOptionsBuilder()
                //唯一标示 保证每个设备都唯一就可以 建议加上GUID
                .WithClientId(uniqueId)
                .WithTcpServer(RabbitMQAddress)
                .WithCredentials(UserName, Password)
                //心跳包默认的发送间隔
                .WithKeepAlivePeriod(TimeSpan.FromSeconds(15))
                //是否清空客户端的连接记录。若为true，则断开后，broker将自动清除该客户端连接信息
                .WithCleanSession()
                .WithCommunicationTimeout(TimeSpan.FromSeconds(10))
                .WithWillMessage(willMsg)
                .Build();

            _client = factory.CreateMqttClient();
            _client.UseConnectedHandler(MqttClient_Connected);
            _client.UseApplicationMessageReceivedHandler(MqttClient_MessageReceived);
            _client.UseDisconnectedHandler(MqttClient_Disconnected);
        }

        public async Task ConnectAsync()
        {
            try
            {
                //await Task.Run(async () => {
                var uniqueId = $"tenant:{_tenantId}-device:{_deviceId}-{_guid}-";
                await _client.ConnectAsync(_options, CancellationToken.None);

                //});

            }
            catch (Exception)
            {
                await Task.Delay(5000);
                await ConnectAsync();
            }
        }

        protected async Task MqttClient_Connected(MqttClientConnectedEventArgs args)
        {
            var topicFilter = new MqttTopicFilterBuilder()
                .WithTopic(_myTopic)
                .WithExactlyOnceQoS()
                .Build();
            // Subscribe to a topic
            await _client.SubscribeAsync(topicFilter);
            // publish to server and set the device is online.
            await _client.PublishAsync(DeviceLogStatusTopic, JsonConvert.SerializeObject(_deviceStatusEvent));
            Console.WriteLine("### SUBSCRIBED ###");
        }

        public async Task MqttClient_Disconnected(MqttClientDisconnectedEventArgs args)
        {
            Console.WriteLine("### DISCONNECTED FROM SERVER ###");
            await Task.Delay(TimeSpan.FromSeconds(5));

            try
            {
                await _client.ConnectAsync(_options, CancellationToken.None); // Since 3.0.5 with CancellationToken
            }
            catch
            {
                Console.WriteLine("### RECONNECTING FAILED ###");
            }
        }

        private void MqttClient_MessageReceived(MqttApplicationMessageReceivedEventArgs e) 
        {
            var message = e.ApplicationMessage;
            var msg = Encoding.UTF8.GetString(message.Payload);
            var obj = JsonConvert.DeserializeObject<JObject>(msg);
            Console.WriteLine(msg);
            OnActionReceived(new ActionReceivedMessageEventArgs { Message = obj });
            //return Task.CompletedTask(null);
        }

        public void OnActionReceived(ActionReceivedMessageEventArgs args)
        {
            ActionMessageReceived?.Invoke(this, args);
        }

        public async Task Publish(string msg, string topic)
        {
            if (_client != null && _client.IsConnected)
            {
                var body = Encoding.UTF8.GetBytes(msg);
                await _client.PublishAsync(topic, body);
            }
        }
    }
}
