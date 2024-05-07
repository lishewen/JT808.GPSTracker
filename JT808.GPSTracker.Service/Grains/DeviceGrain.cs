using JT808.GPSTracker.Common;
using Orleans.Concurrency;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JT808.GPSTracker.Service.Grains
{
    [Reentrant]
    public class DeviceGrain : Grain, IDeviceGrain
    {
        private DeviceMessage _lastMessage = null!;
        private long PackageCount = 0;
        private long DelayPackageCount = 0;
        private readonly IDispatchGrain _dispatcher;
        public DeviceGrain()
        {
            _dispatcher = GrainFactory.GetGrain<IDispatchGrain>(Guid.Empty);
        }
        public async ValueTask ProcessMessage(DeviceMessage message)
        {
            PackageCount++;
            if (DateTime.Now - message.Timestamp > TimeSpan.FromMinutes(5))
            {
                DelayPackageCount++;
            }
            if (_lastMessage is null || _lastMessage.Latitude != message.Latitude || _lastMessage.Longitude != message.Longitude)
            {
                // Only sent a notification if the position has changed
                double speed = GetSpeed(_lastMessage, message);

                // Record the last message
                _lastMessage = message;

                // Forward the message to the notifier grain
                var velocityMessage = new VelocityMessage(message, speed, PackageCount, DelayPackageCount);
                await _dispatcher.Post(velocityMessage);
            }
            else
            {
                // The position has not changed, just record the last message
                _lastMessage = message;
            }
        }
        private static double GetSpeed(DeviceMessage message1, DeviceMessage message2)
        {
            // Calculate the speed of the device, using the internal state of the grain
            if (message1 is null || message2 is null)
            {
                return 0;
            }

            const double R = 6_371 * 1_000;
            double x = (message2.Longitude - message1.Longitude) * Math.Cos((message2.Latitude + message1.Latitude) / 2);
            double y = message2.Latitude - message1.Latitude;
            double distance = Math.Sqrt(Math.Pow(x, 2) + Math.Pow(y, 2)) * R;
            double time = (message2.Timestamp - message1.Timestamp).TotalSeconds;
            return time switch
            {
                0 => 0,
                _ => distance / time,
            };
        }
    }
}
