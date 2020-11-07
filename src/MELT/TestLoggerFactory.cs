using System;
using System.Collections.Generic;
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

        [Obsolete]
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public TestLoggerFactory(ITestSink sink)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddLogging(builder => builder.AddTestLogger(sink));
            _serviceProvider = serviceCollection.BuildServiceProvider();
            _loggerFactory = _serviceProvider.GetService<ILoggerFactory>();
        }

        [Obsolete]
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public IEnumerable<LogEntry> LogEntries => Sink.LogEntries;
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

        [Obsolete]
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public IEnumerable<BeginScope> Scopes => Sink.Scopes;
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

        private TestLoggerFactory(ILoggerFactory loggerFactory, ServiceProvider serviceProvider)
        {
            _loggerFactory = loggerFactory;
            _serviceProvider = serviceProvider;

            // Unfortunately there is a breaking change in 3.1 which causes an exception on BeginScope:
            // System.TypeLoadException : Could not load type 'Microsoft.Extensions.Logging.Abstractions.Internal.NullScope' from assembly 'Microsoft.Extensions.Logging.Abstractions, Version=3.1.0.0, Culture=neutral, PublicKeyToken=adb9793829ddae60'.

            var dependencyVersion = loggerFactory.GetType().Assembly.GetName().Version;
            var abstractionsVersion = typeof(ILoggerFactory).Assembly.GetName().Version;
            if (dependencyVersion.Major < abstractionsVersion.Major)
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
            var loggerFactory = serviceProvider.GetService<ILoggerFactory>();
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
            var loggerFactory = serviceProvider.GetService<ILoggerFactory>();
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
            var loggerFactory = serviceProvider.GetService<ILoggerFactory>();
            return new TestLoggerFactory(loggerFactory, serviceProvider);
        }
    }
}
