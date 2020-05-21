using System.Collections.Generic;
using System.Linq;

namespace MELT
{
    public class SerilogTestLoggerSink : ISerilogTestLoggerSink
    {
        private readonly IInternalTestSink _sink;

        public SerilogTestLoggerSink(IInternalTestSink sink)
        {
            _sink = sink;
        }

        public IEnumerable<SerilogLogEntry> LogEntries => _sink.LogEntries.Select(x => new SerilogLogEntry(x));
    }
}
