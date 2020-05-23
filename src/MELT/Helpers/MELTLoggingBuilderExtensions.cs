using MELT;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;

namespace Microsoft.Extensions.Logging
{
    public static class MELTLoggingBuilderExtensions
    {
        [Obsolete]
        public static ILoggingBuilder AddTestLogger(this ILoggingBuilder builder)
            => builder.AddTestLogger(MELTBuilder.CreateTestSink());

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

        [Obsolete("The recommended alternative is " + nameof(AddTest) + "() or use the simplified factory.WithWebHostBuilder(builder => builder.UseTestLogging()). The sink can then be retrieved with factory.GetTestLoggerSink()")]
        public static ILoggingBuilder AddTestLogger(this ILoggingBuilder builder, ITestSink sink)
        {
            builder.Services.TryAddSingleton(sink);
            return builder.AddProvider(new TestLoggerProvider(sink));
        }

        public static ILoggingBuilder AddTest(this ILoggingBuilder builder)
            => builder.AddTestLoggerProvider(new TestLoggerOptions());

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
            builder.Services.TryAddSingleton<IInternalTestSink>(sink);
            builder.Services.TryAddTransient<ITestLoggerSink, TestLoggerSinkAccessor>();
            return builder.AddProvider(new TestLoggerProvider(sink));
        }
    }
}
