using System;
using MELT;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.Extensions.Logging
{
    /// <summary>
    /// Provides extension methods for adding MELT test logging interceptors to an <see cref="ILoggingBuilder"/>.
    /// </summary>
    public static class MELTLoggingBuilderExtensions
    {
        /// <summary>
        /// Adds a test logger provider to the specified <see cref="ILoggingBuilder"/> with default settings.
        /// </summary>
        /// <param name="builder">The <see cref="ILoggingBuilder"/> to add the provider to.</param>
        /// <returns>The <see cref="ILoggingBuilder"/> for chaining.</returns>
        [Obsolete]
        public static ILoggingBuilder AddTestLogger(this ILoggingBuilder builder)
            => builder.AddTestLogger(MELTBuilder.CreateTestSink());

        /// <summary>
        /// Adds a test logger provider to the specified <see cref="ILoggingBuilder"/> with custom configuration options.
        /// </summary>
        /// <param name="builder">The <see cref="ILoggingBuilder"/> to add the provider to.</param>
        /// <param name="configure">An action to configure the <see cref="TestLoggerOptions"/>.</param>
        /// <returns>The <see cref="ILoggingBuilder"/> for chaining.</returns>
        [Obsolete("The recommended alternative is " + nameof(AddTest) + "(Action<" + nameof(TestLoggerOptions) + ">) or use the simplified factory.WithWebHostBuilder(builder => builder.UseTestLogging()). The sink can then be retrieved with factory.GetTestLoggerSink()")]
        public static ILoggingBuilder AddTestLogger(this ILoggingBuilder builder, Action<TestLoggerOptions> configure)
        {
            if (configure == null) throw new ArgumentNullException(nameof(configure));

            var options = MELTBuilder.CreateOptions(configure);
            return builder.AddTestLogger(MELTBuilder.CreateTestSink(options));
        }

        //public static ILoggingBuilder AddTestLogger(this ILoggingBuilder builder, ITestSink sink)
        //{
        //    builder.Services.TryAddSingleton(sink);
        //    return builder.AddProvider(new TestLoggerProvider(sink));
        //}

        //public static ILoggingBuilder AddTestLogger(this ILoggingBuilder builder, ITestLoggerSink sinkWrapper)
        //{
        //    builder.Services.TryAddSingleton(sinkWrapper.Sink);
        //    return builder.AddProvider(new TestLoggerProvider(sinkWrapper.Sink));
        //}

        /// <summary>
        /// Adds a test logger provider to the specified <see cref="ILoggingBuilder"/> using the provided <see cref="ITestSink"/>.
        /// </summary>
        /// <param name="builder">The <see cref="ILoggingBuilder"/> to add the provider to.</param>
        /// <param name="sink">The <see cref="ITestSink"/> to use for logging.</param>
        /// <returns>The <see cref="ILoggingBuilder"/> for chaining.</returns>
        [Obsolete("The recommended alternative is " + nameof(AddTest) + "() or use the simplified factory.WithWebHostBuilder(builder => builder.UseTestLogging()). The sink can then be retrieved with factory.GetTestLoggerSink()")]
        public static ILoggingBuilder AddTestLogger(this ILoggingBuilder builder, ITestSink sink)
        {
            builder.Services.TryAddSingleton(sink);
            return builder.AddProvider(new TestLoggerProvider(sink));
        }

        /// <summary>
        /// Adds a test logger provider to the specified <see cref="ILoggingBuilder"/> with default options.
        /// </summary>
        /// <param name="builder">The <see cref="ILoggingBuilder"/> to add the provider to.</param>
        /// <returns>The <see cref="ILoggingBuilder"/> for chaining.</returns>
        public static ILoggingBuilder AddTest(this ILoggingBuilder builder)
            => builder.AddTestLoggerProvider(new TestLoggerOptions());

        /// <summary>
        /// Adds a test logger provider to the specified <see cref="ILoggingBuilder"/> with custom configuration options.
        /// </summary>
        /// <param name="builder">The <see cref="ILoggingBuilder"/> to add the provider to.</param>
        /// <param name="configure">An action to configure the <see cref="TestLoggerOptions"/>.</param>
        /// <returns>The <see cref="ILoggingBuilder"/> for chaining.</returns>
        public static ILoggingBuilder AddTest(this ILoggingBuilder builder, Action<TestLoggerOptions> configure)
        {
            if (configure == null) throw new ArgumentNullException(nameof(configure));

            var options = MELTBuilder.CreateOptions(configure);
            return builder.AddTestLoggerProvider(options);
        }

        private static ILoggingBuilder AddTestLoggerProvider(this ILoggingBuilder builder, TestLoggerOptions options)
        {
            var sink = MELTBuilder.CreateTestSink(options);

            builder.Services.TryAddSingleton(sink);
#pragma warning disable CS0612 // Type or member is obsolete
            builder.Services.TryAddSingleton<IInternalTestSink>(sink);
#pragma warning restore CS0612 // Type or member is obsolete
            builder.Services.TryAddTransient<ITestLoggerSink, TestLoggerSinkAccessor>();
#pragma warning disable CS0618 // Type or member is obsolete
            return builder.AddProvider(new TestLoggerProvider(sink));
#pragma warning restore CS0618 // Type or member is obsolete
        }
    }
}
