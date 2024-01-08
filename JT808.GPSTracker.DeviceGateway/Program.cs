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
            UserName = hostContext.Configuration["RabbitMQ:UserName"],//用户名
            Password = hostContext.Configuration["RabbitMQ:Password"],//密码
            HostName = hostContext.Configuration["RabbitMQ:IP"]//rabbitmq ip
        };
        services.AddSingleton<ILoggerFactory, LoggerFactory>();
        services.AddSingleton(typeof(ILogger<>), typeof(Logger<>));
        services.AddSingleton(factory);
        // 使用内存队列实现会话通知
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
