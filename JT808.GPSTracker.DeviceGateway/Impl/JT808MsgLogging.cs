using JT808.Gateway.Abstractions.Enums;
using JT808.Gateway.Abstractions;
using JT808.Protocol.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JT808.GPSTracker.DeviceGateway.Impl
{
    public class JT808MsgLogging(ILoggerFactory loggerFactory) : IJT808MsgLogging
    {
        private readonly ILogger Logger = loggerFactory.CreateLogger("JT808MsgLogging");

        public void Processor((string TerminalNo, byte[] Data) parameter, JT808MsgLoggingType jT808MsgLoggingType)
        {
            Logger.LogDebug($"{jT808MsgLoggingType}-{parameter.TerminalNo}-{parameter.Data.ToHexString()}");
        }
    }
}
