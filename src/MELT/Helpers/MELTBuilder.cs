using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;

namespace MELT
{
    public static class MELTBuilder
    {
        ///// <summary>
        ///// Create a configured test logger sink to be passed to the test logger factory.
        ///// </summary>
        ///// <param name="configure">A delegate used to configure the <see cref="TestLoggerOptions"/>.</param>
        ///// <returns>The test logger sink.</returns>
        //public static ITestLoggerSink CreateLogSink(Action<TestLoggerOptions> configure)
        //{
        //    if (configure == null) throw new ArgumentNullException(nameof(configure));

        //    var options = CreateOptions(configure);
        //    return new TestSink(CreateTestSink(options));
        //}

        ///// <summary>
        ///// Create a default test logger sink to be passed to the test logger factory.
        ///// </summary>
        ///// <returns>The test logger sink.</returns>
        //public static ITestLoggerSink CreateLogSink() => new TestSink(CreateTestSink());

        [Obsolete("The recommended alternative is to not create directly a sink and use factory.WithWebHostBuilder(builder => builder.UseTestLogging())")]
        // <summary>
        /// Create a configured test sink to be passed to the test logger factory.
        /// </summary>
        /// <param name="configure">A delegate used to configure the <see cref="TestLoggerOptions"/>.</param>
        /// <returns>The test logger sink.</returns>
        public static ITestSink CreateTestSink(Action<TestLoggerOptions> configure)
        {
            if (configure == null) throw new ArgumentNullException(nameof(configure));

            var options = CreateOptions(configure);
            return CreateTestSink(options);
        }

        [Obsolete]
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

            return TestLoggerFactory.Create(configure);
        }

        /// <summary>
        /// Create a default logger factory to be used to capture log messages and scopes in a test sink.
        /// </summary>
        /// <param name="configure">A delegate used to configure the <see cref="TestLoggerOptions"/>.</param>
        /// <returns>The test logger factory.</returns>
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

        [Obsolete]
        internal static ITestSink CreateTestSink(TestLoggerOptions options)
        {
            return new TestSink(options.WriteEnabled, options.BeginEnabled);
        }

        //public static ILoggerFactory CreateTestLoggerFactory()
        //{
        //    var serviceCollection = new ServiceCollection();
        //    serviceCollection.AddLogging(builder => builder.AddTest());
        //    var serviceProvider = serviceCollection.BuildServiceProvider();
        //    var loggerFactory = serviceProvider.GetService<ILoggerFactory>();
        //    return new DisposableTestLoggerFactory(loggerFactory, serviceProvider);
        //}

        //public static ILoggerFactory CreateTestLoggerFactory(Action<ILoggingBuilder> configure)
        //{
        //    var serviceCollection = new ServiceCollection();
        //    serviceCollection.AddLogging(builder => builder.AddTest());
        //    serviceCollection.AddLogging(configure);
        //    var serviceProvider = serviceCollection.BuildServiceProvider();
        //    var loggerFactory = serviceProvider.GetService<ILoggerFactory>();
        //    return new DisposableTestLoggerFactory(loggerFactory, serviceProvider);
        //}

        //private class DisposableTestLoggerFactory : ILoggerFactory
        //{
        //    private readonly ILoggerFactory _loggerFactory;

        //    private readonly ServiceProvider _serviceProvider;

        //    public DisposableTestLoggerFactory(ILoggerFactory loggerFactory, ServiceProvider serviceProvider)
        //    {
        //        _loggerFactory = loggerFactory;
        //        _serviceProvider = serviceProvider;
        //    }

        //    public ITestLoggerSink Sink => _serviceProvider.GetRequiredService<ITestLoggerSink>();

        //    public void Dispose()
        //    {
        //        _serviceProvider.Dispose();
        //    }

        //    public ILogger CreateLogger(string categoryName)
        //    {
        //        return _loggerFactory.CreateLogger(categoryName);
        //    }

        //    public void AddProvider(ILoggerProvider provider)
        //    {
        //        _loggerFactory.AddProvider(provider);
        //    }
        //}
    }
}
