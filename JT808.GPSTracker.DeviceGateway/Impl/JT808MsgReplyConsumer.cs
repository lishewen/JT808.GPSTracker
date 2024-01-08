using JT808.Gateway.Abstractions;
using JT808.Protocol.Extensions.Streamax.MessageBody;
using JT808.Protocol;
using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using JT808.Protocol.Extensions;
using JT808.GPSTracker.DeviceGateway.Models;

namespace JT808.GPSTracker.DeviceGateway.Impl
{
    public sealed class JT808MsgReplyConsumer : IJT808MsgReplyConsumer
    {
        private bool disposed = false;
        public CancellationTokenSource Cts { get; private set; } = new CancellationTokenSource();
        private readonly IConnection connection;
        private readonly IModel channel;
        private readonly ILogger logger;
        private readonly JT808Serializer Serializer;
        private readonly ConcurrentQueue<byte[]> queue = new();
        public string TopicName { get; }
        public JT808MsgReplyConsumer(ILoggerFactory loggerFactory, ConnectionFactory factory)
        {
            TopicName = "JT808Message";
            logger = loggerFactory.CreateLogger<JT808MsgReplyConsumer>();
            //创建连接
            connection = factory.CreateConnection();
            //创建通道
            channel = connection.CreateModel();

            channel.QueueDeclare(TopicName, false, false, false, null);

            JT808_Streamax_Config config = new();
            config.Register(Assembly.GetAssembly(typeof(JT808_0x0B04)));
            Serializer = config.GetSerializer();
        }
        private void Dispose(bool disposing)
        {
            if (disposed) return;
            if (disposing)
            {
                connection.Close();
                Cts.Dispose();
            }
            disposed = true;
        }
        public void Dispose()
        {
            //必须为true
            Dispose(true);
            //通知垃圾回收机制不再调用终结器（析构器）
            GC.SuppressFinalize(this);
        }

        public void OnMessage(Action<(string TerminalNo, byte[] Data)> callback)
        {
            Task.Run(() =>
            {
                while (!Cts.IsCancellationRequested)
                {
                    if (disposed) return;
                    try
                    {
                        if (queue.TryDequeue(out var bs))
                        {
                            var message = bs.ToHexString();
                            var package = Serializer.Deserialize(bs);
                            if (logger.IsEnabled(LogLevel.Debug))
                            {
                                logger.LogDebug("Topic: {TopicName} Key: {TerminalPhoneNo} Message: {message}", TopicName, package.Header.TerminalPhoneNo, message);
                            }
                            callback((package.Header.TerminalPhoneNo, bs));
                        }
                    }
                    catch (OperationCanceledException ex)
                    {
                        logger.LogError(ex, TopicName);
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex, TopicName);
                    }
                }
            }, Cts.Token);
        }

        public void Subscribe()
        {
            Console.WriteLine($"JT808MsgReplyConsumer register,routeKey:{TopicName}");
            //channel.ExchangeDeclare(exchange: "message", type: "topic");
            //channel.QueueDeclare(queue: QueueName, exclusive: false);
            //channel.QueueBind(queue: QueueName, exchange: "message", routingKey: RouteKey);
            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (model, ea) =>
            {
                try
                {
                    queue.Enqueue(ea.Body.ToArray());
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "");
                }
                finally
                {
                    channel.BasicAck(ea.DeliveryTag, false);
                }

            };
            channel.BasicConsume(queue: TopicName, consumer: consumer);
        }

        public void Unsubscribe()
        {
            if (disposed) return;
            connection.Close();
            Cts.Cancel();
        }
        ~JT808MsgReplyConsumer()
        {
            Dispose(false);
        }
    }
}
