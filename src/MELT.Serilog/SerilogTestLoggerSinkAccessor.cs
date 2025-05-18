using System.Collections.Generic;
using System.Linq;

namespace MELT
{
#pragma warning disable CS0612 // Type or member is obsolete
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
