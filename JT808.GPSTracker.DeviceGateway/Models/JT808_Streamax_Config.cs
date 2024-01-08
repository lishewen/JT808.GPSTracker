using JT808.Protocol.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JT808.GPSTracker.DeviceGateway.Models
{
    public class JT808_Streamax_Config : GlobalConfigBase
    {
        public override string ConfigId { get; protected set; } = "JT808_Streamax_Config";
    }
}
