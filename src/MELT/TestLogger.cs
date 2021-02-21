// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Extensions.Logging;

namespace MELT
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public class TestLogger : ILogger
    {
#pragma warning disable CS0612 // Type or member is obsolete
        private readonly ITestSink _sink;

        private readonly AsyncLocal<TestScope> _currentScope = new AsyncLocal<TestScope>();

        public TestLogger(string name, ITestSink sink)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            _sink = sink ?? throw new ArgumentNullException(nameof(sink));
        }
#pragma warning restore CS0612 // Type or member is obsolete

        public string Name { get; }

        public IDisposable BeginScope<TState>(TState state)
        {
#pragma warning disable CS0612 // Type or member is obsolete
            _sink.BeginScope(new BeginScopeContext(Name, state));

            var parent = _currentScope.Value;
            var newScope = new TestScope(this, state, parent);
            _currentScope.Value = newScope;

            return newScope;
#pragma warning restore CS0612 // Type or member is obsolete
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (!IsEnabled(logLevel))
            {
                return;
            }

            if (formatter == null) throw new ArgumentNullException(nameof(formatter));

            var message = formatter(state, exception);

            var scopes = GetScopes();

#pragma warning disable CS0612 // Type or member is obsolete
            _sink.Write(new WriteContext(logLevel, eventId, state, exception, _currentScope.Value?.State, Name, message, scopes));
#pragma warning restore CS0612 // Type or member is obsolete
        }

        private IEnumerable<object?> GetScopes()
        {
            var scopes = new Stack<object?>();

            var scope = _currentScope.Value;
            while (scope != null)
            {
                scopes.Push(scope.State);
                scope = scope.Parent;
            }

            return scopes;
        }

        public bool IsEnabled(LogLevel logLevel) => logLevel != LogLevel.None;

        internal class TestScope : IDisposable
        {
            private readonly TestLogger _logger;
            private bool _isDisposed;

            internal TestScope(TestLogger logger, object? state, TestScope parent)
            {
                _logger = logger;
                State = state;
                Parent = parent;
            }

            public TestScope Parent { get; }

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
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}
