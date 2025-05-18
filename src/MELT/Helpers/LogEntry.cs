using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;

namespace MELT
{
    /// <summary>
    /// A captured log entry.
    /// </summary>
    public class LogEntry
    {
#pragma warning disable CS0612 // Type or member is obsolete
        private readonly WriteContext _entry;
        private string? _format;

        internal LogEntry(WriteContext entry)
        {
            _entry = entry;
            Properties = _entry.State as IReadOnlyList<KeyValuePair<string, object>> ?? Constants.EmptyProperties;
        }

        /// <summary>
        /// Id of this log entry.
        /// </summary>
        public EventId EventId => _entry.EventId;

        /// <summary>
        /// Exception included in this log entry. Null if there was no exception.
        /// </summary>
        public Exception? Exception => _entry.Exception;

        /// <summary>
        /// The name of the logger which captured this log entry.
        /// </summary>
        public string LoggerName => _entry.LoggerName;

        /// <summary>
        /// The level used for this log entry.
        /// </summary>
        public LogLevel LogLevel => _entry.LogLevel;

        /// <summary>
        /// The formatted message for this log entry.
        /// </summary>
        public string? Message => _entry.Message;

        /// <summary>
        /// The properties included in this log entry.
        /// </summary>
        public IReadOnlyList<KeyValuePair<string, object>> Properties { get; }

        /// <summary>
        /// The original format of the message for this log entry.
        /// </summary>
        public string OriginalFormat => _format ??= Properties.GetOriginalFormat();

        /// <summary>
        /// The format string for this log entry. Prefer using <see cref="OriginalFormat"/>.
        /// </summary>
        [Obsolete("The preferred alternative is " + nameof(OriginalFormat) + ".")]
        public string Format => OriginalFormat;

        /// <summary>
        /// The inner scope for this log entry.
        /// </summary>
        [Obsolete("The recommended alternative is " + nameof(Scopes) + ".")]
        public Scope Scope => new Scope(_entry.Scope);
#pragma warning restore CS0612 // Type or member is obsolete

        /// <summary>
        /// The scopes for this log entry.
        /// </summary>
        public IEnumerable<Scope> Scopes => _entry.Scopes.Select(s => new Scope(s));
    }
}
