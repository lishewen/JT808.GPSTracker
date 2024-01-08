using JT808.Gateway.Abstractions.Enums;
using JT808.Gateway.Abstractions;
using JT808.Gateway.Services;
using JT808.Protocol.Enums;
using JT808.Protocol.Extensions.Streamax.MessageBody;
using JT808.Protocol.MessageBody;
using JT808.Protocol;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JT808.Protocol.Extensions;
using JT808.GPSTracker.DeviceGateway.Models;

namespace JT808.GPSTracker.DeviceGateway.Impl
{
    public class JT808NormalReplyMessageHandlerImpl : JT808MessageHandler
    {
        private readonly ILogger logger;
        private readonly IJT808MsgLogging jT808MsgLogging;
        private readonly JT808TransmitService jT808TransmitService;
        private readonly IConnection connection;
        private readonly IModel channel;
        public JT808NormalReplyMessageHandlerImpl(
            JT808TransmitService jT808TransmitService,
            IJT808MsgLogging jT808MsgLogging,
            ILoggerFactory loggerFactory,
            IJT808Config jT808Config,
            ConnectionFactory factory) : base(jT808Config)
        {
            //创建连接
            connection = factory.CreateConnection();
            //创建通道
            channel = connection.CreateModel();
            //声明一个队列
            channel.QueueDeclare("BusIO", false, false, false, null);
            channel.QueueDeclare("BusOverSpeed", false, false, false, null);
            channel.QueueDeclare("BusPosition", false, false, false, null);
            channel.QueueDeclare("BusCan", false, false, false, null);
            channel.QueueDeclare("BusActiveSafety", false, false, false, null);
            channel.QueueDeclare("BusFarm", false, false, false, null);
            channel.QueueDeclare("Register", false, false, false, null);
            channel.QueueDeclare("Attendance", false, false, false, null);
            channel.QueueDeclare("ServiceRequest", false, false, false, null);

            this.jT808TransmitService = jT808TransmitService;
            this.jT808MsgLogging = jT808MsgLogging;
            logger = loggerFactory.CreateLogger<JT808NormalReplyMessageHandlerImpl>();

            HandlerDict.Add(0x0B0E, MsgAny);
            HandlerDict.Add(0x0B0B, MsgAny);
            HandlerDict.Add(0x0B08, MsgAny);
            HandlerDict.Add(0x0B10, MsgAny);
            HandlerDict.Add(0x0B11, MsgAny);
            HandlerDict.Add(0x0F0A, MsgAny);
        }
        private byte[] MsgAny(JT808HeaderPackage request)
        {
            logger.LogDebug(MyLogEvents.OriginalData, "OriginalData:{Message}", request.OriginalData.ToArray().ToHexString());
            if (request.Version == JT808Version.JTT2019)
            {
                byte[] data = JT808Serializer.Serialize(JT808MsgId._0x8001.Create_平台通用应答_2019(request.Header.TerminalPhoneNo, new JT808_0x8001()
                {
                    AckMsgId = request.Header.MsgId,
                    JT808PlatformResult = JT808PlatformResult.succeed,
                    MsgNum = request.Header.MsgNum
                }));
                return data;
            }
            else
            {
                byte[] data = JT808Serializer.Serialize(JT808MsgId._0x8001.Create(request.Header.TerminalPhoneNo, new JT808_0x8001()
                {
                    AckMsgId = request.Header.MsgId,
                    JT808PlatformResult = JT808PlatformResult.succeed,
                    MsgNum = request.Header.MsgNum
                }));
                return data;
            }
        }

        /// <summary>
        /// 重写消息处理器
        /// </summary>
        /// <param name="request"></param>
        public override byte[] Processor(in JT808HeaderPackage request)
        {
            //AOP 可以自定义添加一些东西:上下行日志、数据转发
            //logger.LogDebug($"808版本：{request.Version}");
            var parameter = (request.Header.TerminalPhoneNo, request.OriginalData.ToArray());
            //转发数据（可同步也可以使用队列进行异步）
            try
            {
                jT808TransmitService.SendAsync(parameter);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "");
            }
            //上行日志（可同步也可以使用队列进行异步）
            jT808MsgLogging.Processor(parameter, JT808MsgLoggingType.up);
            //处理上行消息
            var down = base.Processor(request);
            //下行日志（可同步也可以使用队列进行异步）
            jT808MsgLogging.Processor((request.Header.TerminalPhoneNo, down), JT808MsgLoggingType.down);
            return down;
        }

        /// <summary>
        /// 重写自带的消息
        /// </summary>
        /// <param name="request"></param>
        public override byte[] Msg0x0200(JT808HeaderPackage request)
        {
            try
            {
                channel.BasicPublish("", "BusPosition", false, null, request.OriginalData.ToArray());
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "");
            }
            return base.Msg0x0200(request);
        }

        ~JT808NormalReplyMessageHandlerImpl()
        {
            channel.Close();
            connection.Close();
        }
    }
}
