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
using JT808.GPSTracker.Common;

namespace JT808.GPSTracker.Consumer.Listeners
{
    public class BusPositionListener : RabbitListener
    {
        private readonly ILogger<BusPositionListener> _logger;
        private readonly IClusterClient _client;
        public BusPositionListener(IOptions<RabbitMQConfiguration> options, ILogger<BusPositionListener> logger, IClusterClient client) : base(options)
        {
            RouteKey = "BusPosition";
            QueueName = "BusPosition";
            _logger = logger;
            _client = client;
        }
        public override bool Process(byte[] bs)
        {
            var message = bs.ToHexString();
            try
            {
                var package = Serializer.Deserialize(bs);
                if (package.Bodies != null)
                {
                    var value = package.Bodies as JT808_0x0200;
                    if (value.Lng > 0 && value.Lat > 0)
                    {
                        int deviceid = Convert.ToInt32(package.Header.TerminalPhoneNo);
                        if (deviceid > 0)
                        {
                            var device = _client.GetGrain<IDeviceGrain>(deviceid);
                            double lng = value.Lng / 1000000d;
                            double lat = value.Lat / 1000000d;
                            DeviceMessage deviceMessage = new(lat, lng, package.Header.MsgNum, deviceid, value.GPSTime);
                            _ = device.ProcessMessage(deviceMessage);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Message:{message}", message);
            }
            return true;
        }
    }
}
