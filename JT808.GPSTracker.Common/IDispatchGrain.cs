using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JT808.GPSTracker.Common
{
    public interface IDispatchGrain : IGrainWithGuidKey
    {
        ValueTask Post(VelocityMessage message);
        ValueTask<List<VelocityMessage>> GetAllRunPoint();
    }
}
