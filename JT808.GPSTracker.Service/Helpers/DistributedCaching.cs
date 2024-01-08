using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JT808.GPSTracker.Service.Helpers
{
    public static class DistributedCaching
    {
        public static async Task SetAsync<T>(this IDistributedCache distributedCache,
                                             string key,
                                             T value,
                                             DistributedCacheEntryOptions options,
                                             CancellationToken token = default)
        {
            ArgumentNullException.ThrowIfNull(distributedCache);

            await distributedCache.SetAsync(key, value.ToByteArray(), options, token).ConfigureAwait(false);
        }

        public static async Task<T> GetAsync<T>(this IDistributedCache distributedCache, string key,
                                                CancellationToken token = default) where T : class
        {
            ArgumentNullException.ThrowIfNull(distributedCache);

            var result = await distributedCache.GetAsync(key, token).ConfigureAwait(false);
            return result.FromByteArray<T>();
        }
    }
}
