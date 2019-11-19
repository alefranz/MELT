// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.Extensions.Logging;
using System;

namespace MELT
{
    public class TestLogger : ILogger
    {
        private readonly ITestSink _sink;
        private readonly Func<LogLevel, bool>? _filter;
        private object? _scope;

        public TestLogger(string name, ITestSink sink, Func<LogLevel, bool>? filter = null)
        {
            _sink = sink ?? throw new ArgumentNullException(nameof(sink));
            Name = name ?? throw new ArgumentNullException(nameof(name));
            _filter = filter;
        }

        public string Name { get; }

        public IDisposable BeginScope<TState>(TState state)
        {
            _scope = state;

            _sink.BeginScope(new BeginScopeContext(Name, state));

            return TestScope.Instance;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (!IsEnabled(logLevel))
            {
                return;
            }

            if (formatter == null) throw new ArgumentNullException(nameof(formatter));

            var message = formatter(state, exception);

            // TODO: scopes should be lazy cached
            _sink.Write(new WriteContext(logLevel, eventId, state, exception, _scope, Name, message));
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            if (logLevel == LogLevel.None) return false;
            return _filter != null ? _filter(logLevel) : true;
        }
    }
}
