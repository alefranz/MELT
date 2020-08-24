using System.Collections.Generic;
using System.Linq;

namespace MELT
{
    internal class TestLoggerSinkAccessor : ITestLoggerSink
    {
#pragma warning disable CS0612 // Type or member is obsolete
        private readonly IInternalTestSink _sink;

        public TestLoggerSinkAccessor(IInternalTestSink sink)
#pragma warning restore CS0612 // Type or member is obsolete
        {
            _sink = sink;
        }

        public IEnumerable<LogEntry> LogEntries => _sink.Writes.Select(x => new LogEntry(x));

        public IEnumerable<BeginScope> Scopes => _sink.BeginScopes.Select(x => new BeginScope(x));

        public void Clear()
        {
            _sink.Clear();
        }
    }
}
