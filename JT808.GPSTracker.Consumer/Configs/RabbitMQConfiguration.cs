using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JT808.GPSTracker.Consumer.Configs
{
    public record RabbitMQConfiguration
    {
        public string IP { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string AppName { get; set; }
    }
}
