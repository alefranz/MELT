using System.Collections.Generic;

namespace MELT
{
    public interface ITestLoggerSink
    {
        IEnumerable<LogEntry> LogEntries { get; }

        IEnumerable<BeginScope> Scopes { get; }

        void Clear();
    }
}
