using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MELT
{
    public interface ISerilogTestLoggerSink
    {
        IEnumerable<SerilogLogEntry> LogEntries { get; }
    }

    public class SerilogLogEntry
    {
        private readonly LogEntry _entry;

        // TODO Use WriteContext ?
        public SerilogLogEntry(LogEntry entry)
        {
            _entry = entry;
        }

        public EventId EventId => _entry.EventId;
        public Exception? Exception => _entry.Exception;
        public string LoggerName => _entry.LoggerName;
        public LogLevel LogLevel => _entry.LogLevel;
        public string? Message => _entry.Message;
        public IEnumerable<KeyValuePair<string, object>> Properties => _entry.Properties;

        public string Format => _entry.Format;

        public SerilogScope Scope => new SerilogScope(_entry.Scope);
    }

    public class SerilogScope : Scope
    {
        public SerilogScope(object? scope) : base(scope)
        {
        }
    }

    public class SerilogTestLoggerSink : ISerilogTestLoggerSink
    {
        private readonly ITestLoggerSink _sink;

        public SerilogTestLoggerSink(ITestLoggerSink sink)
        {
            _sink = sink;
        }

        public IEnumerable<SerilogLogEntry> LogEntries => _sink.LogEntries.Select(x => new SerilogLogEntry(x));
    }

    public static class SerilogExtensions
    {
        public static ISerilogTestLoggerSink AsSerilog(this ITestLoggerSink sink)
        {
            return new SerilogTestLoggerSink(sink);
        }

        public static ITestLoggerSink GetSerilogTestLoggerSink<TStartup>(this WebApplicationFactory<TStartup> factory)
            where TStartup : class
            => GetServices(factory).GetRequiredService<ITestLoggerSink>();
    }
}
