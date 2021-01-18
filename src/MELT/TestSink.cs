// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace MELT
{
    [Obsolete]
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public class TestSink : ITestSink
    {
        private ConcurrentQueue<BeginScopeContext> _beginScopes;
        private ConcurrentQueue<WriteContext> _writes;

        public TestSink(
            Func<WriteContext, bool>? writeEnabled = null,
            Func<BeginScopeContext, bool>? beginEnabled = null)
        {
            WriteEnabled = writeEnabled;
            BeginEnabled = beginEnabled;

            _beginScopes = new ConcurrentQueue<BeginScopeContext>();
            _writes = new ConcurrentQueue<WriteContext>();
        }

        public Func<WriteContext, bool>? WriteEnabled { get; set; }

        public Func<BeginScopeContext, bool>? BeginEnabled { get; set; }

        public IProducerConsumerCollection<BeginScopeContext> BeginScopes { get => _beginScopes; }

        public IProducerConsumerCollection<WriteContext> Writes { get => _writes; }

        public IEnumerable<LogEntry> LogEntries => Writes.Select(x => new LogEntry(x));

        /// <summary>
        /// All scopes that were created
        /// </summary>
        public IEnumerable<BeginScope> Scopes => BeginScopes.Select(x => new BeginScope(x));

        /// <summary>
        /// All the current scopes.
        /// </summary>
        public IEnumerable<BeginScope> CurrentScopeData => _currentScope.ScopeData;

        private (IReadOnlyCollection<BeginScope> ScopeData, IReadOnlyCollection<TestScope> ActiveScopes) _currentScope = (new List<BeginScope>(), new List<TestScope>());

        private readonly object _scopeLock = new { };

        public event Action<WriteContext>? MessageLogged;

        public event Action<BeginScopeContext>? ScopeStarted;

        public void Write(WriteContext context)
        {
            if (WriteEnabled == null || WriteEnabled(context))
            {
                _writes.Enqueue(context);
            }
            MessageLogged?.Invoke(context);
        }

        public IDisposable BeginScope(BeginScopeContext context)
        {
            TestScope testScope;
            lock (_scopeLock)
            {
                var oldScope = _currentScope;
                testScope = new TestScope(this, oldScope);

                //In principle, using Immutable collections here would prevent copying the collections
                //But the number of scopes is low enough that it probably isn't worth the extra dependency
                var newScopeData = oldScope.ScopeData.Prepend(new BeginScope(context)).ToList();

                var newActiveScopes = oldScope.ActiveScopes.Append(testScope).ToList();
                _currentScope = (newScopeData, newActiveScopes);
            }

            if (BeginEnabled == null || BeginEnabled(context))
            {
                _beginScopes.Enqueue(context);
            }
            ScopeStarted?.Invoke(context);

            return testScope;
        }

        public void Clear()
        {
            _beginScopes = new ConcurrentQueue<BeginScopeContext>();
            _writes = new ConcurrentQueue<WriteContext>();
        }

        private void EndScope(TestScope testScope)
        {
            lock (_scopeLock)
            {
                if (_currentScope.ActiveScopes.Contains(testScope))
                    _currentScope = testScope.PreviousScope;
                //Else the test scope had already been exited - we could throw an exception here
            }
        }

        private sealed class TestScope : IDisposable
        {
            //This class is nested so that it can access the EndScope method
            public TestSink TestSink { get; }

            public (IReadOnlyCollection<BeginScope> ScopeData, IReadOnlyCollection<TestScope> ActiveScopes) PreviousScope { get; }

            public TestScope(TestSink testSink, (IReadOnlyCollection<BeginScope> ScopeData, IReadOnlyCollection<TestScope> ActiveScopes) previousScope)
            {
                TestSink = testSink;
                PreviousScope = previousScope;
            }

            private bool _disposed = false;

            /// <inheritdoc />
            public void Dispose()
            {
                if (_disposed) return;
                _disposed = true;
                TestSink.EndScope(this);
            }
        }
    }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}
