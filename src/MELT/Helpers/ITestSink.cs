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

        void Write(WriteContext context);

        void BeginScope(BeginScopeContext context);

        IEnumerable<LogEntry> LogEntries { get; }

        IEnumerable<BeginScope> Scopes { get; }
        
        void Clear();
    }
}
