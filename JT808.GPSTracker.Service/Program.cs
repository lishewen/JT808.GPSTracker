using Orleans.Configuration;
using StackExchange.Redis;
using System.Net;

IConfiguration Configuration = null;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        Configuration = hostContext.Configuration;
        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = Configuration.GetConnectionString("RedisConnection");
            options.ConfigurationOptions.DefaultDatabase = 2;
            options.InstanceName = "BusCache";
        });
    })
    .UseOrleans(builder =>
    {
        var redisConnectionString = Configuration.GetConnectionString("RedisConnection");
        var ServerIP = Configuration.GetSection("Options:ServerIP").Value;
        bool ListenOnAnyHostAddress = Configuration.GetValue<bool>("Options:ListenOnAnyHostAddress");
        var orleansHost = builder.UseRedisClustering(options =>
        {
            options.ConfigurationOptions = ConfigurationOptions.Parse(redisConnectionString);
            options.ConfigurationOptions.DefaultDatabase = 0;
        })
        .UseRedisReminderService(options =>
        {
            options.ConfigurationOptions = ConfigurationOptions.Parse(redisConnectionString);
            options.ConfigurationOptions.DefaultDatabase = 1;
        })
        .AddMemoryGrainStorageAsDefault()
        .Configure<ClusterOptions>(options =>
        {
            options.ClusterId = "JBBus.Orleans";
            options.ServiceId = "JBBus.Orleans";
        })
        .ConfigureEndpoints(IPAddress.Parse(ServerIP), 11111, 30000, ListenOnAnyHostAddress);
    })
    .Build();

await host.RunAsync();
