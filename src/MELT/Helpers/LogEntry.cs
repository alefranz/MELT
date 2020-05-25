using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace MELT
{
    public class LogEntry
    {
        private readonly WriteContext _entry;
        private string? _format;

        public LogEntry(WriteContext entry)
        {
            _entry = entry;
            Properties = _entry.State as IReadOnlyList<KeyValuePair<string, object>> ?? Constants.EmptyProperties;
        }

        public EventId EventId => _entry.EventId;
        public Exception? Exception => _entry.Exception;
        public string LoggerName => _entry.LoggerName;
        public LogLevel LogLevel => _entry.LogLevel;
        public string? Message => _entry.Message;
        public IReadOnlyList<KeyValuePair<string, object>> Properties { get; }

        public string OriginalFormat => _format ??= GetFormat();

        [Obsolete()]
        public string Format => _format ??= GetFormat();

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

        public Scope Scope => new Scope(_entry.Scope);
    }
}
