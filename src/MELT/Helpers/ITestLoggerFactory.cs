using Microsoft.Extensions.Logging;
using System.Collections.Generic;

namespace MELT
{
    public interface ITestLoggerFactory : ILoggerFactory
    {
        IEnumerable<LogEntry> LogEntries { get; }
        IEnumerable<Scope> Scopes { get; }
    }
}
