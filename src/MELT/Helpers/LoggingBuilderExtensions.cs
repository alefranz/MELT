using MELT;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;

namespace Microsoft.Extensions.Logging
{
    public static class LoggingBuilderExtensions
    {
        public static ILoggingBuilder AddTestLogger(this ILoggingBuilder builder)
            => builder.AddTestLogger(MELTBuilder.CreateTestSink());

        public static ILoggingBuilder AddTestLogger(this ILoggingBuilder builder, Action<TestSinkOptions> configure)
            => builder.AddTestLogger(MELTBuilder.CreateTestSink(configure));

        public static ILoggingBuilder AddTestLogger(this ILoggingBuilder builder, ITestSink sink)
        {
            builder.Services.TryAddSingleton(sink);
            return builder.AddProvider(new TestLoggerProvider(sink));
        }
    }
}
