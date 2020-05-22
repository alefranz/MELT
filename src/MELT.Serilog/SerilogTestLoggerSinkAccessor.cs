using System.Collections.Generic;
using System.Linq;

namespace MELT.Serilog
{
    internal class SerilogTestLoggerSinkAccessor : ISerilogTestLoggerSink
    {
        private readonly IInternalTestSink _sink;

        public SerilogTestLoggerSinkAccessor(IInternalTestSink sink)
        {
            _sink = sink;
        }

        public IEnumerable<SerilogLogEntry> LogEntries => _sink.Writes.Select(x => new SerilogLogEntry(x));

        public void Clear() => _sink.Clear();
    }
}
