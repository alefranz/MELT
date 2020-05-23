using Microsoft.Extensions.Logging;
using Serilog.Events;
using System;
using System.Collections.Generic;

namespace MELT.Serilog
{
    public class SerilogLogEntry
    {
        private static readonly IReadOnlyDictionary<string, object> EmptyDictionary = new Dictionary<string, object>();
        private readonly WriteContext _writeContext;

        public SerilogLogEntry(WriteContext writeContext)
        {
            _writeContext = writeContext;
        }

        public EventId EventId => _writeContext.EventId;
        public Exception? Exception => _writeContext.Exception;
        public string LoggerName => _writeContext.LoggerName;
        public LogEventLevel LogLevel => (LogEventLevel)_writeContext.LogLevel;
        public string? Message => _writeContext.Message;
        public IReadOnlyList<KeyValuePair<string, object>> Properties => _writeContext.State as IReadOnlyList<KeyValuePair<string, object>> ?? Constants.EmptyProperties;

        // Serilog put the format at the end
        public string Format => Properties.Count > 0 && Properties[Properties.Count - 1].Key == "{OriginalFormat}" && Properties[Properties.Count - 1].Value is string s ? s : Constants.NullString;

        public SerilogScope Scope => new SerilogScope(_writeContext.Scope);
    }
}
