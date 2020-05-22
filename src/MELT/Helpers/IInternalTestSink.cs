using System;
using System.Collections.Concurrent;

namespace MELT
{
    [Obsolete]
    public interface IInternalTestSink
    {
        //event Action<WriteContext>? MessageLogged;

        //event Action<BeginScopeContext>? ScopeStarted;

        Func<WriteContext, bool>? WriteEnabled { get; set; }

        Func<BeginScopeContext, bool>? BeginEnabled { get; set; }

        IProducerConsumerCollection<BeginScopeContext> BeginScopes { get; }

        IProducerConsumerCollection<WriteContext> Writes { get; }

        void Write(WriteContext context);

        void BeginScope(BeginScopeContext context);

        void Clear();
    }
}
