using System.Collections.Generic;
using System.Linq;

namespace MELT
{
    internal class TestLoggerSinkAccessor : ITestLoggerSink
    {
        private readonly IInternalTestSink _sink;

        public TestLoggerSinkAccessor(IInternalTestSink sink)
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
