using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JT808.GPSTracker.Common
{
    [Immutable, GenerateSerializer]
    public record class DeviceMessage(
        double Latitude,
        double Longitude,
        long MessageId,
        int DeviceId,
        DateTime Timestamp);
}
