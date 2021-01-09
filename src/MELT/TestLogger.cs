// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.Extensions.Logging;

namespace MELT
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public class TestLogger : ILogger
    {
#pragma warning disable CS0612 // Type or member is obsolete
        private readonly ITestSink _sink;

        /// <summary>
        /// The most recent scope, even if it has been exited.
        /// </summary>
        [Obsolete]
        private object? _scope;

        public TestLogger(string name, ITestSink sink)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            _sink = sink ?? throw new ArgumentNullException(nameof(sink));
        }
#pragma warning restore CS0612 // Type or member is obsolete

        public string Name { get; }

        public IDisposable BeginScope<TState>(TState state)
        {
            _scope = state;
            return _sink.BeginScope(new BeginScopeContext(Name, state));
        }



        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (!IsEnabled(logLevel))
            {
                return;
            }

            if (formatter == null) throw new ArgumentNullException(nameof(formatter));

            var message = formatter(state, exception);

#pragma warning disable CS0612 // Type or member is obsolete
            _sink.Write(new WriteContext(logLevel, eventId, state, exception, _scope, Name, message, _sink.CurrentScopeData));
#pragma warning restore CS0612 // Type or member is obsolete
        }

        public bool IsEnabled(LogLevel logLevel) => logLevel != LogLevel.None;


    }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}
