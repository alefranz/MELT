using System;

namespace MELT
{
    [Obsolete]
    public interface ITestSink : IInternalTestSink, ITestLoggerSink
    {
        new void Clear();
    }
}
