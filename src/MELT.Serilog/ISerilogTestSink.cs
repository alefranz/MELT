using System.Collections.Generic;

namespace MELT
{
    public interface ISerilogTestLoggerSink
    {
        IEnumerable<SerilogLogEntry> LogEntries { get; }
    }
}
