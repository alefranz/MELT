using System.Collections.Generic;

namespace MELT
{
    /// <summary>
    /// Represents a test logger sink for capturing Serilog log entries during testing.
    /// </summary>
    public interface ISerilogTestLoggerSink
    {
        /// <summary>
        /// Gets the collection of captured Serilog log entries.
        /// </summary>
        IEnumerable<SerilogLogEntry> LogEntries { get; }

        /// <summary>
        /// Clears all captured log entries.
        /// </summary>
        void Clear();
    }
}
