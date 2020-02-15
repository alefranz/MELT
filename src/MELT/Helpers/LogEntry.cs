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
            var properties = new Dictionary<string, object>();

            if (_entry.State is IEnumerable<KeyValuePair<string, object>> propertiesList)
            {
                foreach (var prop in propertiesList)
                {
                    if (prop.Key == "{OriginalFormat}")
                    {
                        Format = prop.Value as string;
                        continue;
                    }

                    properties[prop.Key] = prop.Value;
                }
            }

            Properties = properties;
        }

        public EventId EventId => _entry.EventId;
        public Exception? Exception => _entry.Exception;
        public string LoggerName => _entry.LoggerName;
        public LogLevel LogLevel => _entry.LogLevel;
        public string? Message => _entry.Message;
        public IReadOnlyDictionary<string, object> Properties { get; }
        public string? Format { get; }
        public Scope Scope => new Scope(_entry.Scope);
    }
}
