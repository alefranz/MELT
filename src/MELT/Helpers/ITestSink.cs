using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace MELT
{
    [Obsolete]
    public interface ITestSink : IInternalTestSink, ITestLoggerSink
    { }

    public interface ITestLoggerSink
    {
        IEnumerable<LogEntry> LogEntries { get; }

        IEnumerable<BeginScope> Scopes { get; }

        void Clear();
    }

    [Obsolete]
    public interface IInternalTestSink
    {
        event Action<WriteContext>? MessageLogged;

        event Action<BeginScopeContext>? ScopeStarted;

        Func<WriteContext, bool>? WriteEnabled { get; set; }

        Func<BeginScopeContext, bool>? BeginEnabled { get; set; }

        IProducerConsumerCollection<BeginScopeContext> BeginScopes { get; }

        IProducerConsumerCollection<WriteContext> Writes { get; }

        void Write(WriteContext context);

        void BeginScope(BeginScopeContext context);
    }
}
