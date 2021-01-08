// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
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
        public ImmutableStack<object?> CurrentScope { get; private set; } = ImmutableStack<object?>.Empty;


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
            if (BeginEnabled == null || BeginEnabled(context))
            {
                _beginScopes.Enqueue(context);
            }
            ScopeStarted?.Invoke(context);

            var oldScope = CurrentScope;
            CurrentScope = CurrentScope.Push(context.Scope);
            return new TestScope(this, oldScope);
        }



        public void Clear()
        {
            _beginScopes = new ConcurrentQueue<BeginScopeContext>();
            _writes = new ConcurrentQueue<WriteContext>();
        }


        private void EndScope(ImmutableStack<object?> previousScope)
        {
            var previousScopeCount = previousScope.Count();
            //TODO also dispose of all scopes move deeply nested than this one to prevent this possibility
            Debug.Assert(previousScopeCount < CurrentScope.Count(), "Scopes were exited in the wrong order");
            Debug.Assert(previousScope.Reverse().SequenceEqual(CurrentScope.Reverse().Take(previousScopeCount)));

            CurrentScope = previousScope;
        }

        private sealed class TestScope : IDisposable
        {
            public TestSink TestSink { get; }

            public ImmutableStack<object?> PreviousScope { get; }
            //This class is nested so that it can access the EndScope method

            public TestScope(TestSink testSink, ImmutableStack<object?> previousScope)
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
                TestSink.EndScope(PreviousScope);
            }
        }
    }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}
