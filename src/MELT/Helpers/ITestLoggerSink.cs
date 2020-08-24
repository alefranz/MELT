using System.Collections.Generic;

namespace MELT
{
    /// <summary>
    /// Represent a type used to get access to the captured logs and scopes.
    /// </summary>
    public interface ITestLoggerSink
    {
        /// <summary>
        /// The captured logs.
        /// </summary>
        IEnumerable<LogEntry> LogEntries { get; }

        /// <summary>
        /// The captured scopes.
        /// </summary>
        IEnumerable<BeginScope> Scopes { get; }

        /// <summary>
        /// Clear the captured logs and scopes.
        /// </summary>
        void Clear();
    }
}
