using System;
using System.Collections.Generic;
using System.Diagnostics;
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
#pragma warning disable CS0612 // Type or member is obsolete
        private readonly WriteContext _writeContext;
#pragma warning restore CS0612 // Type or member is obsolete
        private static readonly IReadOnlyList<LogEventPropertyValue> _emptyProperties = new LogEventPropertyValue[0];
        private string? _format;
        private IReadOnlyList<LogEventPropertyValue>? _scope;

        /// <summary>
        /// Initializes a new instance of the <see cref="SerilogLogEntry"/> class.
        /// </summary>
        /// <param name="writeContext">The write context containing log information.</param>
        [Obsolete]
        public SerilogLogEntry(WriteContext writeContext)
        {
            _writeContext = writeContext;
        }

        /// <summary>
        /// Gets the event ID for this log entry.
        /// </summary>
        public EventId EventId => _writeContext.EventId;

        /// <summary>
        /// Gets the exception associated with this log entry, if any.
        /// </summary>
        public Exception? Exception => _writeContext.Exception;

        /// <summary>
        /// Gets the name of the logger that created this log entry.
        /// </summary>
        public string LoggerName => _writeContext.LoggerName;

        /// <summary>
        /// Gets the log level of this log entry.
        /// </summary>
        public LogLevel LogLevel => _writeContext.LogLevel;

        /// <summary>
        /// Gets the formatted log message.
        /// </summary>
        public string? Message => _writeContext.Message;

        /// <summary>
        /// Gets the properties associated with this log entry.
        /// </summary>
        public IReadOnlyList<KeyValuePair<string, object>> Properties => _writeContext.State as IReadOnlyList<KeyValuePair<string, object>> ?? Constants.EmptyProperties;

        /// <summary>
        /// Gets the original format of the log message before parameter substitution.
        /// </summary>
        public string OriginalFormat => _format ??= GetFormat();
        private string GetFormat()
        {
            foreach (var prop in Properties)
            {
                if (prop.Key == "{OriginalFormat}")
                {
                    if (prop.Value is string value) return value;

                    Debug.Assert(false, "{OriginalFormat} should always be string.");
                    return Constants.NullString;
                }
            }

            Debug.Assert(false, "{OriginalFormat} should always be present.");
            return Constants.NullString;
        }

        /// <summary>
        /// Gets the scope values from Serilog log entry.
        /// </summary>
        /// <remarks>
        /// This property is obsolete. Use <see cref="Scopes"/> instead.
        /// </remarks>
        [Obsolete("The recommended alternative is " + nameof(Scopes) + ".")]
        public IReadOnlyList<LogEventPropertyValue> Scope => _scope ??= GetSerilogScopes();

        /// <summary>
        /// Gets the scope values from Serilog log entry.
        /// </summary>
        public IReadOnlyList<LogEventPropertyValue> Scopes => _scope ??= GetSerilogScopes();

        private IReadOnlyList<LogEventPropertyValue> GetSerilogScopes()
        {
            var scopeSequence = Properties.SingleOrDefault(x => x.Key == "Scope").Value as SequenceValue;
            return scopeSequence?.Elements ?? _emptyProperties;
        }
    }
}
