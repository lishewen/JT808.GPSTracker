using JT808.GPSTracker.BlazorWeb.Client.Pages;
using JT808.GPSTracker.BlazorWeb.Components;
using Orleans.Configuration;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseOrleansClient((context, client) =>
{
    var redisConnectionString = context.Configuration.GetConnectionString("RedisConnection");
    client.UseRedisClustering(options =>
    {
        options.ConfigurationOptions = ConfigurationOptions.Parse(redisConnectionString!);
        options.ConfigurationOptions.DefaultDatabase = 0;
    })
    .Configure<ClusterOptions>(options =>
    {
        options.ClusterId = "JBBus.Orleans";
        options.ServiceId = "JBBus.Orleans";
    });
});

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(Counter).Assembly);

app.Run();
