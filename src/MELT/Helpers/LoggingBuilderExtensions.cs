using MELT;
using System;

namespace Microsoft.Extensions.Logging
{
    public static class LoggingBuilderExtensions
    {
        public static ILoggingBuilder AddTestLogger(this ILoggingBuilder builder) =>
            builder.AddTestLogger(new TestSink());

        public static ILoggingBuilder AddTestLogger(this ILoggingBuilder builder, ITestSink sink) =>
            builder.AddProvider(new TestLoggerProvider(sink));
    }
}
