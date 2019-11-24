using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace MELT
{
    public class LogEntry
    {
        private readonly WriteContext _entry;

        public LogEntry(WriteContext entry)
        {
            _entry = entry;
        }

        public EventId EventId => _entry.EventId;
        public Exception? Exception => _entry.Exception;
        public string LoggerName => _entry.LoggerName;
        public LogLevel LogLevel => _entry.LogLevel;
        public string? Message => _entry.Message;
        public IEnumerable<KeyValuePair<string, object>> Properties => _entry.State as IEnumerable<KeyValuePair<string, object>> ?? Constants.EmptyProperties;
        public Scope Scope => new Scope(_entry.Scope);
    }
}
