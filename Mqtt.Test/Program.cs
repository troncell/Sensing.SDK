using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mqtt.Test
{
    class Program
    {
        public const string ExchangeName = "amq.topic";
        public const string DeviceControllerTopic = "sensing.device.controller";
        public const string DeviceLogStatusTopic = "sensing.device.log.status";
        public static string RabbitMQAddress = "139.224.23.171";
        public static string UserName = "troncell";
        public static string Password = "1qazTronCell@WSX";

        static void Main(string[] args)
        {

            // UseMqttTest();

            //var automapperTest = new AutomapperTest();
            //automapperTest.MapperStart();

            //ConnectionFactory factory = new ConnectionFactory();
            //// "guest"/"guest" by default, limited to localhost connections
            //factory.UserName = "troncell";
            //factory.Password = password;
            //factory.VirtualHost("/");
            //factory.HostName = hostName;
            //factory.setPort(portNumber);
            //IConnection conn = factory.CreateConnection();

            Sensing.Mqtt.SensingStoreMqttClient client = new Sensing.Mqtt.SensingStoreMqttClient("45", "77");
            client.ConnectAsync();

            Console.ReadLine();

            ConnectionMultiplexer redis = ConnectionMultiplexer.Connect("139.224.23.171:6379,defaultDatabase=0,password=troncell-redis");

            //int databaseNumber = 1
            //object asyncState = 
            IDatabase db = redis.GetDatabase();
            db.StringSet("name", "william");


            string value = db.StringGet("name");
            Console.WriteLine(value);

        }


        public static void UseMqttTest()
        {
            var factory = new ConnectionFactory() { HostName = RabbitMQAddress, UserName = UserName, Password = Password };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.ExchangeDeclare(exchange: ExchangeName, type: "topic", true);


                var arguments = new Dictionary<string, object>
                    {
                        {"x-message-ttl", 60000}
                    };
                var queueName = channel.QueueDeclare("Cap.sensingstore", durable: true, exclusive: false, autoDelete: false, arguments: arguments).QueueName;

                //var queueName = channel.QueueDeclare(")
                //.QueueName;




                channel.QueueBind(queue: queueName,
                                       exchange: ExchangeName,
                                       routingKey: DeviceLogStatusTopic);


                Console.WriteLine(" [*] Waiting for messages. To exit press CTRL+C");

                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (model, ea) =>
                {
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);
                    var routingKey = ea.RoutingKey;
                    Console.WriteLine(" [x] Received '{0}':'{1}'",
                                      routingKey,
                                      message);
                    channel.BasicAck(ea.DeliveryTag, true);
                };
                channel.BasicConsume(queue: queueName,
                                     autoAck: false,
                                     consumer: consumer);

                Console.WriteLine(" Press [enter] to exit.");
                Console.ReadLine();
            }
        }
    }
}
