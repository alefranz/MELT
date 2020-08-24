using System;
using System.Collections.Concurrent;

namespace MELT
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
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
