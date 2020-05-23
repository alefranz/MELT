using MELT;
using System;

namespace Microsoft.Extensions.Logging
{
    public static class MELTLoggerFactoryExtensions
    {
        [Obsolete]
        public static ITestLoggerSink AddTest(this ILoggerFactory loggerFactory)
        {
            var sink = MELTBuilder.CreateTestSink();
            loggerFactory.AddProvider(new TestLoggerProvider(sink));
            return sink;
        }

        [Obsolete]
        public static ITestLoggerSink AddTest(this ILoggerFactory loggerFactory, Action<TestLoggerOptions> configure)
        {
            var sink = MELTBuilder.CreateTestSink(configure);
            loggerFactory.AddProvider(new TestLoggerProvider(sink));
            return sink;
        }

        public static ITestLoggerSink GetTestLoggerSink(this ILoggerFactory loggerFactory)
        {
            if (loggerFactory is TestLoggerFactory disposableTestLoggerFactory) return disposableTestLoggerFactory.Sink;

            throw new ArgumentException($"The {nameof(ILoggerFactory)} has not be created with {nameof(TestLoggerFactory)}.{nameof(TestLoggerFactory.Create)}().", nameof(loggerFactory));
        }
    }
}
