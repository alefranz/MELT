using System;

namespace MELT
{
    public static class MELTBuilder
    {
        /// <summary>
        /// Create a configured test sink to be passed to the test logger factory.
        /// </summary>
        /// <param name="configure">A delegate used to configure the <see cref="TestLoggerOptions"/>.</param>
        /// <returns>The test sink.</returns>
        public static ITestSink CreateTestSink(Action<TestLoggerOptions> configure)
        {
            if (configure == null) throw new ArgumentNullException(nameof(configure));

            var options = CreateOptions(configure);
            return CreateTestSink(options);
        }

        /// <summary>
        /// Create a default test sink to be passed to the test logger factory.
        /// </summary>
        /// <returns>The test sink.</returns>
        public static ITestSink CreateTestSink() => new TestSink();

        /// <summary>
        /// Create a configured logger factory to be used to capture log messages and scopes in a test sink.
        /// </summary>
        /// <param name="configure">A delegate used to configure the <see cref="TestLoggerOptions"/>.</param>
        /// <param name="useScopeFromProperties">Indicate if it should attempt to retrieve the scope from the properties, e.g. when using Serilog. It defaults to false.</param>
        /// <returns>The test logger factory.</returns>
        public static ITestLoggerFactory CreateLoggerFactory(Action<TestLoggerOptions> configure)
        {
            if (configure == null) throw new ArgumentNullException(nameof(configure));

            var options = CreateOptions(configure);
            var sink = CreateTestSink(options);
            return new TestLoggerFactory(sink, options.UseScopeFromProperties);
        }

        /// <summary>
        /// Create a default logger factory to be used to capture log messages and scopes in a test sink.
        /// </summary>
        /// <param name="configure">A delegate used to configure the <see cref="TestLoggerOptions"/>.</param>
        /// <returns>The test logger factory.</returns>
        public static ITestLoggerFactory CreateLoggerFactory()
        {
            var sink = CreateTestSink();
            return new TestLoggerFactory(sink, false);
        }

        internal static TestLoggerOptions CreateOptions(Action<TestLoggerOptions> configure)
        {
            var options = new TestLoggerOptions();
            configure(options);
            return options;
        }

        internal static TestSink CreateTestSink(TestLoggerOptions options)
        {
            return new TestSink(options.WriteEnabled, options.BeginEnabled);
        }
    }
}
