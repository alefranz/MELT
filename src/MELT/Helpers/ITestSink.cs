using System;

namespace MELT
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    [Obsolete]
    public interface ITestSink : IInternalTestSink, ITestLoggerSink
    {
        new void Clear();
    }
}
