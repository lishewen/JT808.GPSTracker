using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace JT808.GPSTracker.Service.Helpers
{
    public static class BinarySerialization
    {
        public static byte[] ToByteArray(this object obj)
        {
            if (obj == null)
            {
                return null;
            }
            return Encoding.UTF8.GetBytes(JsonSerializer.Serialize(obj));
        }
        public static T FromByteArray<T>(this byte[] byteArray) where T : class
        {
            if (byteArray == null)
            {
                return null;
            }
            return JsonSerializer.Deserialize<T>(Encoding.UTF8.GetString(byteArray));
        }
    }
}
