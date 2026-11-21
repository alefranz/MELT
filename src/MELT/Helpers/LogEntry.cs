using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace MELT
{
    /// <summary>
    /// A captured log entry.
    /// </summary>
    public class LogEntry
    {
        private string? _format;

        internal LogEntry(
            LogLevel logLevel,
            EventId eventId,
            object? state,
            Exception? exception,
            string loggerName,
            string? message,
            IReadOnlyList<Scope> scopes)
        {
            LogLevel = logLevel;
            EventId = eventId;
            Exception = exception;
            LoggerName = loggerName;
            Message = message;
            Properties = state as IReadOnlyList<KeyValuePair<string, object>> ?? Constants.EmptyProperties;
            Scopes = scopes;
        }

        /// <summary>
        /// Id of this log entry.
        /// </summary>
        public EventId EventId { get; }

        /// <summary>
        /// Exception included in this log entry. Null if there was no exception.
        /// </summary>
        public Exception? Exception { get; }

        /// <summary>
        /// The name of the logger which captured this log entry.
        /// </summary>
        public string LoggerName { get; }

        /// <summary>
        /// The level used for this log entry.
        /// </summary>
        public LogLevel LogLevel { get; }

        /// <summary>
        /// The formatted message for this log entry.
        /// </summary>
        public string? Message { get; }

        /// <summary>
        /// The properties included in this log entry.
        /// </summary>
        public IReadOnlyList<KeyValuePair<string, object>> Properties { get; }

        /// <summary>
        /// The original format of the message for this log entry.
        /// </summary>
        public string OriginalFormat => _format ??= Properties.GetOriginalFormat();

        /// <summary>
        /// The scopes for this log entry.
        /// </summary>
        public IEnumerable<Scope> Scopes { get; }
    }
}
