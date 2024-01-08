using JT808.GPSTracker.Consumer.Configs;
using JT808.GPSTracker.Consumer.Listeners;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        services.Configure<RabbitMQConfiguration>(hostContext.Configuration.GetSection("RabbitMQConfiguration"));
        services.AddHostedService<BusPositionListener>();
    })
    .Build();

await host.RunAsync();
