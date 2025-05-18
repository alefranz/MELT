using System;
using MELT;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.Extensions.Logging
{
    /// <summary>
    /// Extension methods for setting up test logging services in an <see cref="ILoggingBuilder" />.
    /// </summary>
    public static class MELTSerilogLoggingBuilderExtensions
    {
        /// <summary>
        /// Adds a provider for testing logs with the Serilog behaviour to the <see cref="ILoggingBuilder"/>
        /// </summary>
        /// <param name="builder">The <see cref="ILoggingBuilder"/> to add it to.</param>
        /// <returns></returns>
        public static ILoggingBuilder AddSerilogTest(this ILoggingBuilder builder)
            => builder.AddSerilogTestLoggerProvider(new TestLoggerOptions());

        /// <summary>
        /// Adds a provider for testing logs with the Serilog behaviour to the <see cref="ILoggingBuilder"/>
        /// </summary>
        /// <param name="builder">The <see cref="ILoggingBuilder"/> to add it to.</param>
        /// <param name="configure">The delegate to configure the <see cref="TestLoggerOptions"/>.</param>
        /// <returns>The <see cref="ILoggingBuilder"/> so that additional calls can be chained.</returns>
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
#pragma warning disable CS0612 // Type or member is obsolete
            builder.Services.TryAddSingleton<IInternalTestSink>(sink);
#pragma warning restore CS0612 // Type or member is obsolete
            builder.Services.TryAddTransient<ISerilogTestLoggerSink, SerilogTestLoggerSinkAccessor>();
#pragma warning disable CS0618 // Type or member is obsolete
            return builder.AddProvider(new TestLoggerProvider(sink));
#pragma warning restore CS0618 // Type or member is obsolete
        }
    }
}
