using MELT;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;

namespace Microsoft.Extensions.Logging
{
    public static class LoggingBuilderExtensions
    {
        public static ILoggingBuilder AddTestLogger(this ILoggingBuilder builder)
            => builder.AddTestLogger(MELTBuilder.CreateTestSink());

        public static ILoggingBuilder AddTestLogger(this ILoggingBuilder builder, Action<TestLoggerOptions> configure)
        {
            if (configure == null) throw new ArgumentNullException(nameof(configure));

            var options = MELTBuilder.CreateOptions(configure);
            return builder.AddTestLogger(MELTBuilder.CreateTestSink(options));
        }

        public static ILoggingBuilder AddTestLogger(this ILoggingBuilder builder, ITestSink sink)
        {
            builder.Services.TryAddSingleton(sink);
            return builder.AddProvider(new TestLoggerProvider(sink));
        }
    }
}
