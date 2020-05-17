// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace MELT
{
    internal class TestSink : ITestSink
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

        //[Obsolete]
        public IEnumerable<LogEntry> LogEntries => Writes.Select(x => new LogEntry(x));

        //[Obsolete]
        public IEnumerable<BeginScope> Scopes => BeginScopes.Select(x => new BeginScope(x));

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

        public void BeginScope(BeginScopeContext context)
        {
            if (BeginEnabled == null || BeginEnabled(context))
            {
                _beginScopes.Enqueue(context);
            }
            ScopeStarted?.Invoke(context);
        }

        //[Obsolete]
        public void Clear()
        {
            _beginScopes = new ConcurrentQueue<BeginScopeContext>();
            _writes = new ConcurrentQueue<WriteContext>();
        }
    }

    //public class LogSink : IHaveSink
    //{
    //    private readonly ITestSink _sink;

    //    ITestSink IHaveSink.Sink => _sink;

    //    public LogSink(ITestSink sink)
    //    {
    //        _sink = sink;
    //    }

    //    public IEnumerable<LogEntry> LogEntries => _sink.Writes.Select(x => new LogEntry(x));
    //    public IEnumerable<BeginScope> Scopes => _sink.BeginScopes.Select(x => new BeginScope(x));

    //    public void Clear() => _sink.Clear();
    //}

    //internal interface IHaveSink
    //{
    //    internal ITestSink Sink { get; }
    //}
}
