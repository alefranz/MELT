using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace MELT
{
    public interface ITestLoggerFactory : ILoggerFactory
    {
        [Obsolete]
        IEnumerable<LogEntry> LogEntries { get; }

        [Obsolete]
        IEnumerable<BeginScope> Scopes { get; }

        ITestLoggerSink Sink { get; }
    }
}
