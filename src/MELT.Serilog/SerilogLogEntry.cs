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
        public IReadOnlyDictionary<string, object> Properties => _writeContext.State as IReadOnlyDictionary<string, object> ?? EmptyDictionary;

        public string Format => Properties.TryGetValue("", out var value) && value is string format ? format : Constants.NullString;

        public SerilogScope Scope => new SerilogScope(_writeContext.Scope);
    }
}
