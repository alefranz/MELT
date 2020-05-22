using System.Collections.Generic;

namespace MELT.Serilog
{
    public interface ISerilogTestLoggerSink
    {
        IEnumerable<SerilogLogEntry> LogEntries { get; }

        void Clear();
    }
}
