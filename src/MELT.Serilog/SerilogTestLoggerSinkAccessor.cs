using System.Collections.Generic;
using System.Linq;

namespace MELT
{
    internal class SerilogTestLoggerSinkAccessor : ISerilogTestLoggerSink
    {
        private readonly TestLoggerSink _sink;

        public SerilogTestLoggerSinkAccessor(TestLoggerSink sink)
        {
            _sink = sink;
        }

        public IEnumerable<SerilogLogEntry> LogEntries => _sink.LogEntries.Select(x => new SerilogLogEntry(x));

        public void Clear() => _sink.Clear();
    }
}
