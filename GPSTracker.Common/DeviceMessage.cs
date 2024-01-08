using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPSTracker.Common
{
    [Immutable, GenerateSerializer]
    public record class DeviceMessage(
        double Latitude,
        double Longitude,
        long MessageId,
        string DeviceId,
        DateTime Timestamp);
}
