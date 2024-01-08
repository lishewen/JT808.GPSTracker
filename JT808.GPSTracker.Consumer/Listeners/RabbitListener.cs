using JT808.GPSTracker.Consumer.Configs;
using JT808.Protocol;
using JT808.Protocol.Extensions.Streamax.MessageBody;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace JT808.GPSTracker.Consumer.Listeners
{
    public class RabbitListener : IHostedService
    {
        protected JT808Serializer Serializer;
        private readonly IConnection connection;
        protected readonly IModel channel;

        public RabbitListener(IOptions<RabbitMQConfiguration> options)
        {
            try
            {
                var factory = new ConnectionFactory()
                {
                    // 这是我这边的配置,自己改成自己用就好
                    HostName = options.Value.IP,
                    UserName = options.Value.UserName,
                    Password = options.Value.Password,
                };
                connection = factory.CreateConnection();
                channel = connection.CreateModel();

                JT808_Streamax_Config config = new();
                config.Register(Assembly.GetAssembly(typeof(JT808_0x0B04)));
                Serializer = config.GetSerializer();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"RabbitListener init error,ex:{ex.Message}");
            }
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            Register();
            return Task.CompletedTask;
        }

        protected string RouteKey;
        protected string QueueName;

        // 处理消息的方法
        public virtual bool Process(byte[] bs)
        {
            throw new NotImplementedException();
        }

        // 注册消费者监听在这里
        public void Register()
        {
            Console.WriteLine($"RabbitListener register,routeKey:{RouteKey}");
            //channel.ExchangeDeclare(exchange: "message", type: "topic");
            //channel.QueueDeclare(queue: QueueName, exclusive: false);
            //channel.QueueBind(queue: QueueName, exchange: "message", routingKey: RouteKey);
            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (model, ea) =>
            {
                var result = Process(ea.Body.ToArray());
                if (result)
                {
                    channel.BasicAck(ea.DeliveryTag, false);
                }
            };
            channel.BasicConsume(queue: QueueName, consumer: consumer);
        }

        public void DeRegister()
        {
            connection.Close();
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            connection.Close();
            return Task.CompletedTask;
        }
    }
}
