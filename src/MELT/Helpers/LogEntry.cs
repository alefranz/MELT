using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

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

        public EventId EventId => _entry.EventId;
        public Exception? Exception => _entry.Exception;
        public string LoggerName => _entry.LoggerName;

        /// <summary>
        /// The level used for this entry.
        /// </summary>
        public LogLevel LogLevel => _entry.LogLevel;

        /// <summary>
        /// The formatted message for this entry.
        /// </summary>
        public string? Message => _entry.Message;


        public IReadOnlyList<KeyValuePair<string, object>> Properties { get; }

        public string OriginalFormat => _format ??= Properties.GetOriginalFormat();

        [Obsolete("The preferred alternative is " + nameof(OriginalFormat) + ".")]
        public string Format => OriginalFormat;

        public Scope Scope => new Scope(_entry.Scope);
#pragma warning restore CS0612 // Type or member is obsolete
    }
}
