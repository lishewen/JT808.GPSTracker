using JT808.GPSTracker.Consumer.Configs;
using JT808.Protocol.Extensions.Streamax.MessageBody;
using JT808.Protocol.Extensions;
using JT808.Protocol.MessageBody;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace JT808.GPSTracker.Consumer.Listeners
{
    public class BusPositionListener : RabbitListener
    {
        private readonly ILogger<BusPositionListener> _logger;
        private readonly RabbitMQConfiguration option;
        public BusPositionListener(IOptions<RabbitMQConfiguration> options, ILogger<BusPositionListener> logger) : base(options)
        {
            RouteKey = "BusPosition";
            QueueName = "BusPosition";
            _logger = logger;
            option = options?.Value;
        }
        public override bool Process(byte[] bs)
        {
            return true;
        }
    }
}
