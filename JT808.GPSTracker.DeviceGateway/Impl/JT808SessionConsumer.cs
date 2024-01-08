using JT808.Gateway.Abstractions;
using JT808.GPSTracker.DeviceGateway.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JT808.GPSTracker.DeviceGateway.Impl
{
    public class JT808SessionConsumer(
        JT808SessionService jT808SessionService,
        ILoggerFactory loggerFactory) : IJT808SessionConsumer
    {
        public CancellationTokenSource Cts => new();

        private readonly ILogger logger = loggerFactory.CreateLogger("JT808SessionConsumer");

        public string TopicName { get; } = JT808GatewayConstants.SessionTopic;

        public void OnMessage(Action<(string Notice, string TerminalNo)> callback)
        {
            Task.Run(async () =>
            {
                while (!Cts.IsCancellationRequested)
                {
                    try
                    {
                        var item = await jT808SessionService.ReadAsync(Cts.Token);
                        callback(item);
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex, "");
                    }
                }
            }, Cts.Token);
        }

        public void Unsubscribe()
        {
            Cts.Cancel();
        }

        public void Dispose() => Cts.Dispose();

        public void Subscribe()
        {

        }
    }
}
