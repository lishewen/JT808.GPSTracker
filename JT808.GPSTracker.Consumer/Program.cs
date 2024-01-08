using JT808.GPSTracker.Consumer.Configs;
using JT808.GPSTracker.Consumer.Listeners;
using Microsoft.Extensions.Configuration;
using Orleans.Configuration;
using StackExchange.Redis;

IHost host = Host.CreateDefaultBuilder(args)
    .UseOrleansClient((context, client) =>
    {
        var redisConnectionString = context.Configuration.GetConnectionString("RedisConnection");
        client.UseRedisClustering(options =>
        {
            options.ConfigurationOptions = ConfigurationOptions.Parse(redisConnectionString);
            options.ConfigurationOptions.DefaultDatabase = 0;
        })
        .Configure<ClusterOptions>(options =>
        {
            options.ClusterId = "JBBus.Orleans";
            options.ServiceId = "JBBus.Orleans";
        });
    })
    .ConfigureServices((hostContext, services) =>
    {
        services.Configure<RabbitMQConfiguration>(hostContext.Configuration.GetSection("RabbitMQConfiguration"));
        services.AddHostedService<BusPositionListener>();
    })
    .Build();

await host.RunAsync();
