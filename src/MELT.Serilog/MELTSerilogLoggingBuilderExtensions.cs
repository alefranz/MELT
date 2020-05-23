using MELT;
using MELT.Serilog;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;

namespace Microsoft.Extensions.Logging
{
    public static class MELTSerilogLoggingBuilderExtensions
    {
        public static ILoggingBuilder AddSerilogTest(this ILoggingBuilder builder)
            => builder.AddSerilogTestLoggerProvider(new TestLoggerOptions());

        public static ILoggingBuilder AddSerilogTest(this ILoggingBuilder builder, Action<TestLoggerOptions> configure)
        {
            if (configure == null) throw new ArgumentNullException(nameof(configure));

            var options = MELTBuilder.CreateOptions(configure);
            return builder.AddSerilogTestLoggerProvider(options);
        }

        private static ILoggingBuilder AddSerilogTestLoggerProvider(this ILoggingBuilder builder, TestLoggerOptions options)
        {
            var sink = MELTBuilder.CreateTestSink(options);

            builder.Services.TryAddSingleton(sink);
            builder.Services.TryAddSingleton<IInternalTestSink>(sink);
            builder.Services.TryAddTransient<ISerilogTestLoggerSink, SerilogTestLoggerSinkAccessor>();
            return builder.AddProvider(new TestLoggerProvider(sink));
        }
    }
}
