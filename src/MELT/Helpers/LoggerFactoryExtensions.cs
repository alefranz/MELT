using MELT;
using System;

namespace Microsoft.Extensions.Logging
{
    public static class LoggerFactoryExtensions
    {
        public static ITestLoggerSink AddTest(this ILoggerFactory loggerFactory)
        {
            var sink = MELTBuilder.CreateTestSink();
            loggerFactory.AddProvider(new TestLoggerProvider(sink));
            return sink;
        }

        public static ITestLoggerSink AddTest(this ILoggerFactory loggerFactory, Action<TestLoggerOptions> configure)
        {
            var sink = MELTBuilder.CreateTestSink(configure);
            loggerFactory.AddProvider(new TestLoggerProvider(sink));
            return sink;
        }
    }
}
