using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace MELT
{
    public interface ITestSink
    {
        event Action<WriteContext> MessageLogged;

        event Action<BeginScopeContext> ScopeStarted;

        Func<WriteContext, bool> WriteEnabled { get; set; }

        Func<BeginScopeContext, bool> BeginEnabled { get; set; }

        IProducerConsumerCollection<BeginScopeContext> BeginScopes { get; set; }

        IProducerConsumerCollection<WriteContext> Writes { get; set; }

        IEnumerable<LogEntry> LogEntries { get; }

        IEnumerable<BeginScope> Scopes { get; }
        
        void Write(WriteContext context);

        void Begin(BeginScopeContext context);
    }
}
