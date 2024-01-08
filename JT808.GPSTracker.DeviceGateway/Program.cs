using JT808.Gateway;
using JT808.Gateway.Abstractions;
using JT808.Gateway.Extensions;
using JT808.GPSTracker.DeviceGateway.Impl;
using JT808.GPSTracker.DeviceGateway.Services;
using JT808.Protocol;
using JT808.Protocol.Extensions.Streamax;
using RabbitMQ.Client;

using IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        ConnectionFactory factory = new()
        {
            UserName = hostContext.Configuration["RabbitMQ:UserName"],//�û���
            Password = hostContext.Configuration["RabbitMQ:Password"],//����
            HostName = hostContext.Configuration["RabbitMQ:IP"]//rabbitmq ip
        };
        services.AddSingleton<ILoggerFactory, LoggerFactory>();
        services.AddSingleton(typeof(ILogger<>), typeof(Logger<>));
        services.AddSingleton(factory);
        // ʹ���ڴ����ʵ�ֻỰ֪ͨ
        services.AddSingleton<JT808SessionService>();
        services.AddSingleton<IJT808SessionProducer, JT808SessionProducer>();
        services.AddSingleton<IJT808SessionConsumer, JT808SessionConsumer>();
        services.AddJT808Configure()
                .AddStreamaxConfigure()
                .AddGateway(hostContext.Configuration)
                .AddMessageHandler<JT808NormalReplyMessageHandlerImpl>()
                .AddMsgReplyConsumer<JT808MsgReplyConsumer>()
                .AddMsgLogging<JT808MsgLogging>()
                .AddSessionNotice()
                .AddTransmit(hostContext.Configuration)
                .AddTcp()
                .AddUdp()
                .Builder();
    })
    .UseConsoleLifetime()
    .Build();

await host.StartAsync();
