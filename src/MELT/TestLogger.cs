// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Extensions.Logging;

namespace MELT
{
    /// <summary>
    /// An <see cref="ILogger"/> implementation that captures log messages and scopes in a <see cref="TestLoggerSink"/>.
    /// </summary>
    public class TestLogger : ILogger
    {
        private readonly TestLoggerSink _sink;
        private readonly AsyncLocal<TestScope?> _currentScope = new AsyncLocal<TestScope?>();

        internal TestLogger(string name, TestLoggerSink sink)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            _sink = sink ?? throw new ArgumentNullException(nameof(sink));
        }

        /// <summary>
        /// Gets the category name of this logger.
        /// </summary>
        public string Name { get; }

        /// <inheritdoc/>
        public IDisposable? BeginScope<TState>(TState state)
            where TState : notnull
        {
            _sink.BeginScope(new BeginScope(Name, state));

            var parent = _currentScope.Value;
            var newScope = new TestScope(this, state, parent);
            _currentScope.Value = newScope;

            return newScope;
        }

        /// <inheritdoc/>
        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
            if (!IsEnabled(logLevel))
            {
                return;
            }

            if (formatter == null) throw new ArgumentNullException(nameof(formatter));

            var message = formatter(state, exception);

            var scopes = GetScopes();

            _sink.Write(new LogEntry(logLevel, eventId, state, exception, Name, message, scopes));
        }

        private IReadOnlyList<Scope> GetScopes()
        {
            var current = _currentScope.Value;
            if (current == null)
            {
                return Array.Empty<Scope>();
            }

            var count = 0;
            for (var cursor = current; cursor != null; cursor = cursor.Parent)
            {
                count++;
            }

            var scopes = new Scope[count];
            var scope = current;
            for (var i = count - 1; i >= 0; i--)
            {
                scopes[i] = new Scope(scope!.State);
                scope = scope.Parent;
            }

            return scopes;
        }

        /// <inheritdoc/>
        public bool IsEnabled(LogLevel logLevel) => logLevel != LogLevel.None;

        internal class TestScope : IDisposable
        {
            private readonly TestLogger _logger;
            private bool _isDisposed;

            internal TestScope(TestLogger logger, object? state, TestScope? parent)
            {
                _logger = logger;
                State = state;
                Parent = parent;
            }

            public TestScope? Parent { get; }

            public object? State { get; }

            public void Dispose()
            {
                if (!_isDisposed)
                {
                    _logger._currentScope.Value = Parent;
                    _isDisposed = true;
                }
            }
        }
    }
}
