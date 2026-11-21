using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using Serilog.Events;

namespace MELT
{
    /// <summary>
    /// Represents a log entry captured from Serilog during testing.
    /// </summary>
    public class SerilogLogEntry
    {
        private readonly LogEntry _logEntry;
        private IReadOnlyList<LogEventPropertyValue>? _scopes;

        internal SerilogLogEntry(LogEntry logEntry)
        {
            _logEntry = logEntry;
        }

        /// <summary>
        /// Gets the event ID for this log entry.
        /// </summary>
        public EventId EventId => _logEntry.EventId;

        /// <summary>
        /// Gets the exception associated with this log entry, if any.
        /// </summary>
        public Exception? Exception => _logEntry.Exception;

        /// <summary>
        /// Gets the name of the logger that created this log entry.
        /// </summary>
        public string LoggerName => _logEntry.LoggerName;

        /// <summary>
        /// Gets the log level of this log entry.
        /// </summary>
        public LogLevel LogLevel => _logEntry.LogLevel;

        /// <summary>
        /// Gets the formatted log message.
        /// </summary>
        public string? Message => _logEntry.Message;

        /// <summary>
        /// Gets the properties associated with this log entry.
        /// </summary>
        public IReadOnlyList<KeyValuePair<string, object>> Properties => _logEntry.Properties;

        /// <summary>
        /// Gets the original format of the log message before parameter substitution.
        /// </summary>
        public string OriginalFormat => _logEntry.OriginalFormat;

        /// <summary>
        /// Gets the scope values from Serilog log entry.
        /// </summary>
        public IReadOnlyList<LogEventPropertyValue> Scopes => _scopes ??= GetSerilogScopes();

        private IReadOnlyList<LogEventPropertyValue> GetSerilogScopes()
        {
            var scopeSequence = Properties.SingleOrDefault(x => x.Key == "Scope").Value as SequenceValue;
            return scopeSequence?.Elements ?? Array.Empty<LogEventPropertyValue>();
        }
    }
}
