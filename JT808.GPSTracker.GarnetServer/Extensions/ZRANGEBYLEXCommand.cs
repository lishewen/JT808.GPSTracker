using Garnet.common;
using Garnet.server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JT808.GPSTracker.GarnetServer.Extensions
{
    public class ZRANGEBYLEXCommand : CustomTransactionProcedure
    {
        public override void Main<TGarnetApi>(TGarnetApi api, ArgSlice input, ref MemoryResult<byte> output) => throw new InvalidOperationException();

        public override bool Prepare<TGarnetReadApi>(TGarnetReadApi api, ArgSlice input) => false;

        public override void Finalize<TGarnetApi>(TGarnetApi api, ArgSlice input, ref MemoryResult<byte> output)
        {
            int offset = 0;
            var key = GetNextArg(input, ref offset);
            var min = GetNextArg(input, ref offset);
            var max = GetNextArg(input, ref offset);
            api.SortedSetRange(key, min, max, SortedSetOrderOperation.ByLex, out var outputvalues, out _);

            WriteBulkStringArray(ref output, outputvalues);
        }
    }
}
