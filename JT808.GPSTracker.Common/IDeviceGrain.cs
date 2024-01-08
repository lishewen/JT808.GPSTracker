using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JT808.GPSTracker.Common
{
    public interface IDeviceGrain : IGrainWithStringKey
    {
        ValueTask ProcessMessage(DeviceMessage message);
    }
}
