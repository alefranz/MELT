using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace MELT
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public static class MELTBuilder
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    {
        /// <summary>
        /// Create a configured test sink to be passed to the test logger factory.
        /// </summary>
        /// <param name="configure">A delegate used to configure the <see cref="TestLoggerOptions"/>.</param>
        /// <returns>The test logger sink.</returns>
        [Obsolete("The recommended alternative is to not create directly a sink and use factory.WithWebHostBuilder(builder => builder.UseTestLogging(Action<TestLoggerOptions>)). The sink can then be retrieved with factory.GetTestLoggerSink()")]
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
        [Obsolete("The recommended alternative is to not create directly a sink and use factory.WithWebHostBuilder(builder => builder.UseTestLogging()). The sink can then be retrieved with factory.GetTestLoggerSink()")]
        public static ITestSink CreateTestSink() => new TestSink();

        /// <summary>
        /// Create a configured logger factory to be used to capture log messages and scopes in a test sink.
        /// </summary>
        /// <param name="configure">A delegate used to configure the <see cref="TestLoggerOptions"/>.</param>
        /// <returns>The test logger factory.</returns>
        [Obsolete("The recommended alternative is " + nameof(TestLoggerFactory) + "." + nameof(TestLoggerFactory.Create) + "(Action<" + nameof(TestLoggerOptions) + ">)")]
        public static ITestLoggerFactory CreateLoggerFactory(Action<TestLoggerOptions> configure)
        {
            if (configure == null) throw new ArgumentNullException(nameof(configure));

            return TestLoggerFactory.Create(configure);
        }

        /// <summary>
        /// Create a default logger factory to be used to capture log messages and scopes in a test sink.
        /// </summary>
        /// <returns>The test logger factory.</returns>
        [Obsolete("The recommended alternative is " + nameof(TestLoggerFactory) + "." + nameof(TestLoggerFactory.Create) + "()")]
        public static ITestLoggerFactory CreateLoggerFactory() => TestLoggerFactory.Create();

        ///// <summary>
        ///// Create a default logger factory to be used to capture log messages and scopes in a test sink.
        ///// </summary>
        ///// <param name="configure">A delegate used to configure the <see cref="TestLoggerOptions"/>.</param>
        ///// <returns>The test logger factory.</returns>
        //public static ITestLoggerFactory CreateTestLoggerFactory()
        //{
        //    return new TestLoggerFactory(new TestSink());
        //}

        internal static TestLoggerOptions CreateOptions(Action<TestLoggerOptions> configure)
        {
            var options = new TestLoggerOptions();
            configure(options);
            return options;
        }

#pragma warning disable CS0612 // Type or member is obsolete
        internal static ITestSink CreateTestSink(TestLoggerOptions options) => new TestSink(options.WriteEnabled, options.BeginEnabled);
#pragma warning restore CS0612 // Type or member is obsolete

    }
}
