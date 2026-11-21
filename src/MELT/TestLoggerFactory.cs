using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace MELT
{
    /// <summary>
    /// A logger factory to be used to capture log messages and scopes in a test sink.
    /// </summary>
    public class TestLoggerFactory : ITestLoggerFactory
    {
        private readonly ILoggerFactory _loggerFactory;
        private readonly ServiceProvider _serviceProvider;

        private TestLoggerFactory(ILoggerFactory loggerFactory, ServiceProvider serviceProvider)
        {
            _loggerFactory = loggerFactory;
            _serviceProvider = serviceProvider;

            // A consumer can resolve Logging below Logging.Abstractions when it pins packages independently.
            var dependencyVersion = loggerFactory.GetType().Assembly.GetName().Version;
            var abstractionsVersion = typeof(ILoggerFactory).Assembly.GetName().Version;
            if (dependencyVersion is not null && abstractionsVersion is not null && dependencyVersion.Major < abstractionsVersion.Major)
            {
                var version = $"{abstractionsVersion.Major}.{abstractionsVersion.Minor}.x";
                throw new MissingNuGetPackageException($"\nWhen testing a project which reference\nMicrosoft.Extensions.Logging.Abstractions version {version}\n"
                    + $"the test project must include a reference to\nMicrosoft.Extensions.Logging version {version}");
            }
        }

        /// <summary>
        /// The <see cref="ITestLoggerSink"/> which gives access to the captured logs.
        /// </summary>
        public ITestLoggerSink Sink => _serviceProvider.GetRequiredService<ITestLoggerSink>();

        /// <inheritdoc/>
        public void Dispose()
        {
            _serviceProvider.Dispose();
        }

        /// <inheritdoc/>
        public ILogger CreateLogger(string categoryName)
        {
            return _loggerFactory.CreateLogger(categoryName);
        }

        /// <inheritdoc/>
        public void AddProvider(ILoggerProvider provider)
        {
            _loggerFactory.AddProvider(provider);
        }

        /// <summary>
        /// Create a default logger factory to be used to capture log messages and scopes in a test sink.
        /// </summary>
        /// <returns>The test logger factory.</returns>
        public static ITestLoggerFactory Create()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddLogging(builder => builder.AddTest().SetMinimumLevel(LogLevel.Trace));
            var serviceProvider = serviceCollection.BuildServiceProvider();
            var loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();
            return new TestLoggerFactory(loggerFactory, serviceProvider);
        }

        /// <summary>
        /// Create a configured logger factory to be used to capture log messages and scopes in a test sink.
        /// </summary>
        /// <param name="configure">A delegate used to configure the <see cref="ILoggingBuilder"/>.</param>
        /// <returns>The test logger factory.</returns>
        public static ITestLoggerFactory Create(Action<ILoggingBuilder> configure)
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddLogging(builder => builder.AddTest().SetMinimumLevel(LogLevel.Trace));
            serviceCollection.AddLogging(configure);
            var serviceProvider = serviceCollection.BuildServiceProvider();
            var loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();
            return new TestLoggerFactory(loggerFactory, serviceProvider);
        }

        /// <summary>
        /// Create a configured logger factory to be used to capture log messages and scopes in a test sink.
        /// </summary>
        /// <param name="configure">A delegate used to configure the <see cref="TestLoggerOptions"/>.</param>
        /// <returns>The test logger factory.</returns>
        public static ITestLoggerFactory Create(Action<TestLoggerOptions> configure)
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddLogging(builder => builder.AddTest(configure).SetMinimumLevel(LogLevel.Trace));
            var serviceProvider = serviceCollection.BuildServiceProvider();
            var loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();
            return new TestLoggerFactory(loggerFactory, serviceProvider);
        }
    }
}
