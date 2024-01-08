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
        private readonly IDispatchGrain _dispatcher;
        public DeviceGrain() {
            _dispatcher = GrainFactory.GetGrain<IDispatchGrain>(Guid.Empty);
        }
        public async ValueTask ProcessMessage(VelocityMessage message)
        {
            await _dispatcher.Post(message);
        }
    }
}
