using Microsoft.Extensions.Logging;
using Serilog.Events;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace MELT
{
    public class SerilogLogEntry
    {
        private readonly WriteContext _writeContext;
        private static readonly IReadOnlyList<LogEventPropertyValue> _emptyProperties = new LogEventPropertyValue[0];
        private string? _format;
        private IReadOnlyList<LogEventPropertyValue>? _scope;

        public SerilogLogEntry(WriteContext writeContext)
        {
            _writeContext = writeContext;
        }

        public EventId EventId => _writeContext.EventId;
        public Exception? Exception => _writeContext.Exception;
        public string LoggerName => _writeContext.LoggerName;
        public LogLevel LogLevel => _writeContext.LogLevel;
        public string? Message => _writeContext.Message;
        public IReadOnlyList<KeyValuePair<string, object>> Properties => _writeContext.State as IReadOnlyList<KeyValuePair<string, object>> ?? Constants.EmptyProperties;

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

        public IReadOnlyList<LogEventPropertyValue> Scope => _scope ??= GetSerilogScope();

        private IReadOnlyList<LogEventPropertyValue> GetSerilogScope()
        {
            var scopeSequence = Properties.SingleOrDefault(x => x.Key == "Scope").Value as SequenceValue;
            return scopeSequence?.Elements ?? _emptyProperties;
        }
    }
}
