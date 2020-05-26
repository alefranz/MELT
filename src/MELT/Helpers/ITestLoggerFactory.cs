using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace MELT
{
    public interface ITestLoggerFactory : ILoggerFactory
    {
        [Obsolete("The recommended alternative is " + nameof(Sink) + "." + nameof(ITestLoggerSink.LogEntries) + ".")]
        IEnumerable<LogEntry> LogEntries { get; }

        [Obsolete("The recommended alternative is " + nameof(Sink) + "." + nameof(ITestLoggerSink.Scopes) + ".")]
        IEnumerable<BeginScope> Scopes { get; }

        ITestLoggerSink Sink { get; }
    }
}
