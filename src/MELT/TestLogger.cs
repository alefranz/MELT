// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.Extensions.Logging;

namespace MELT
{
    public class TestLogger : ILogger
    {
        private readonly ITestSink _sink;
        private object? _scope;

        public TestLogger(string name, ITestSink sink)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            _sink = sink ?? throw new ArgumentNullException(nameof(sink));
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

            _sink.Write(new WriteContext(logLevel, eventId, state, exception, _scope, Name, message));
        }

        public bool IsEnabled(LogLevel logLevel) => logLevel != LogLevel.None;
    }

    //public class SerilogTestLogger : ILogger
    //{
    //    private readonly ITestSink _sink;
    //    private readonly Func<LogLevel, bool>? _filter;

    //    public SerilogTestLogger(string name, ITestSink sink, Func<LogLevel, bool>? filter = null)
    //    {
    //        _sink = sink ?? throw new ArgumentNullException(nameof(sink));
    //        Name = name ?? throw new ArgumentNullException(nameof(name));
    //        _filter = filter;
    //    }

    //    public string Name { get; }

    //    public IDisposable BeginScope<TState>(TState state) => TestScope.Instance;

    //    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
    //    {
    //        if (!IsEnabled(logLevel))
    //        {
    //            return;
    //        }

    //        if (formatter == null) throw new ArgumentNullException(nameof(formatter));

    //        var message = formatter(state, exception);

    //        object? scope = null;

    //        if (state is IEnumerable<KeyValuePair<string, object>> properties)
    //        {
    //            foreach (var prop in properties)
    //            {
    //                if (prop.Key == "Scope")
    //                {
    //                    scope = prop.Value;
    //                    //if (prop.Value is Serilog.Events.LogEventPropertyValue scopes)
    //                    //{
    //                    //    scope = scopes;
    //                    //}

    //                    break;
    //                }
    //            }
    //        }

    //        _sink.Write(new WriteContext(logLevel, eventId, state, exception, scope ?? _scope, Name, message));
    //    }

    //    public bool IsEnabled(LogLevel logLevel)
    //    {
    //        if (logLevel == LogLevel.None) return false;
    //        return _filter != null ? _filter(logLevel) : true;
    //    }
    //}
}
