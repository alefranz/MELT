using MELT;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SampleWebApplicationSerilogAlternate.Tests
{
    public interface ISerilogTestSink
    {
        IEnumerable<SerilogLogEntry> LogEntries { get; }
    }

    public class SerilogLogEntry
    {
        private readonly LogEntry _entry;

        public SerilogLogEntry(LogEntry entry)
        {
            _entry = entry;
        }

        public EventId EventId => _entry.EventId;
        public Exception? Exception => _entry.Exception;
        public string LoggerName => _entry.LoggerName;
        public LogLevel LogLevel => _entry.LogLevel;
        public string? Message => _entry.Message;
        public IEnumerable<KeyValuePair<string, object>> Properties => _entry.Properties as 

        public string Format => _entry.Format;

        public SerilogScope Scope => new SerilogScope(_entry.Scope);
    }

    public class SerilogScope : Scope
    {
        public SerilogScope(object? scope) : base(scope)
        {
        }
    }

    public class SerilogTestSink : ISerilogTestSink
    {
        private readonly ITestSink _sink;

        public SerilogTestSink(ITestSink sink)
        {
            _sink = sink;
        }

        public IEnumerable<SerilogLogEntry> LogEntries => _sink.LogEntries.Select(x => new SerilogLogEntry(x));
    }
}
