using System;

namespace MELT
{
    public static class MELTBuilder
    {
        /// <summary>
        /// Create a configured test sink to be passed to the test logger factory.
        /// </summary>
        /// <param name="configure">A delegate used to configure the <see cref="TestSinkOptions"/>.</param>
        /// <returns>The test sink.</returns>
        public static ITestSink CreateTestSink(Action<TestSinkOptions> configure)
        {
            if (configure == null) throw new ArgumentNullException(nameof(configure));

            var options = new TestSinkOptions();
            configure(options);
            return new TestSink(options.WriteEnabled, options.BeginEnabled);
        }

        /// <summary>
        /// Create a default test sink to be passed to the test logger factory.
        /// </summary>
        /// <returns>The test sink.</returns>
        public static ITestSink CreateTestSink() => new TestSink();

        /// <summary>
        /// Create a configured logger factory to be used to capture log messages and scopes in a test sink.
        /// </summary>
        /// <param name="configure">A delegate used to configure the <see cref="TestSinkOptions"/>.</param>
        /// <returns>The test logger factory.</returns>
        public static ITestLoggerFactory CreateLoggerFactory(Action<TestSinkOptions> configure)
        {
            var sink = CreateTestSink(configure);
            return new TestLoggerFactory(sink);
        }

        /// <summary>
        /// Create a default logger factory to be used to capture log messages and scopes in a test sink.
        /// </summary>
        /// <param name="configure">A delegate used to configure the <see cref="TestSinkOptions"/>.</param>
        /// <returns>The test logger factory.</returns>
        public static ITestLoggerFactory CreateLoggerFactory()
        {
            var sink = CreateTestSink();
            return new TestLoggerFactory(sink);
        }
    }
}
