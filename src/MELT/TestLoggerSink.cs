using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace MELT
{
    internal sealed class TestLoggerSink : ITestLoggerSink
    {
        private readonly Func<LogEntry, bool>? _writeEnabled;
        private readonly Func<BeginScope, bool>? _beginEnabled;
        private volatile ConcurrentQueue<BeginScope> _scopes = new ConcurrentQueue<BeginScope>();
        private volatile ConcurrentQueue<LogEntry> _logEntries = new ConcurrentQueue<LogEntry>();

        public TestLoggerSink()
        {
        }

        public TestLoggerSink(TestLoggerOptions options)
        {
            if (options == null) throw new ArgumentNullException(nameof(options));

            _writeEnabled = options.WriteEnabled;
            _beginEnabled = options.BeginEnabled;
        }

        public IEnumerable<LogEntry> LogEntries => _logEntries;

        public IEnumerable<BeginScope> Scopes => _scopes;

        public void Write(LogEntry entry)
        {
            if (_writeEnabled == null || _writeEnabled(entry))
            {
                _logEntries.Enqueue(entry);
            }
        }

        public void BeginScope(BeginScope scope)
        {
            if (_beginEnabled == null || _beginEnabled(scope))
            {
                _scopes.Enqueue(scope);
            }
        }

        public void Clear()
        {
            _scopes = new ConcurrentQueue<BeginScope>();
            _logEntries = new ConcurrentQueue<LogEntry>();
        }
    }
}
