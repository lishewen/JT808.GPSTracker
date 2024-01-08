using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPSTracker.Common
{
    [Immutable, GenerateSerializer]
    public record VelocityMessage(
        DeviceMessage DeviceMessage,
        double Velocity) :
         DeviceMessage(
            DeviceMessage.Latitude,
            DeviceMessage.Longitude,
            DeviceMessage.MessageId,
            DeviceMessage.DeviceId,
            DeviceMessage.Timestamp);

    [Immutable, GenerateSerializer]
    public record class VelocityBatch(List<VelocityMessage> Messages);
}
