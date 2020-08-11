using MELT;
using System;

namespace Microsoft.Extensions.Logging
{
    public static class MELTLoggerFactoryExtensions
    {
        public static ITestLoggerSink GetTestLoggerSink(this ILoggerFactory loggerFactory)
        {
            if (loggerFactory is TestLoggerFactory testLoggerFactory) return testLoggerFactory.Sink;

            throw new ArgumentException($"The {nameof(ILoggerFactory)} has not be created with {nameof(TestLoggerFactory)}.{nameof(TestLoggerFactory.Create)}().", nameof(loggerFactory));
        }
    }
}
