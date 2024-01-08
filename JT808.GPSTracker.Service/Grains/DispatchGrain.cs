using JT808.GPSTracker.Common;
using JT808.GPSTracker.Service.Helpers;
using Microsoft.Extensions.Caching.Distributed;
using Orleans.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JT808.GPSTracker.Service.Grains
{
    public class DispatchGrain(IDistributedCache distributedCache) : Grain, IDispatchGrain
    {
        private readonly Dictionary<int, VelocityMessage> _buses = [];
        public async ValueTask<List<VelocityMessage>> GetAllRunPoint()
        {
            string key = "AllRunPoint";
            List<VelocityMessage> list = await distributedCache.GetAsync<List<VelocityMessage>>(key);
            if (list == null)
            {
                list = [.. _buses.Values];
                await distributedCache.SetAsync(key,
                    list,
                    new() { AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(5) });
            }
            return list;
        }

        public ValueTask Post(VelocityMessage message)
        {
            _buses[message.DeviceId] = message;
            return default;
        }
    }
}
