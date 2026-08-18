using System;
using MELT;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.AspNetCore.Mvc.Testing
{
    /// <summary>
    /// Extension methods for <see cref="WebApplicationFactory{TStartup}"/> to work with test logging capabilities.
    /// </summary>
    public static class MELTWebApplicationFactoryExtensions
    {
        /// <summary>
        /// Tries to get the <see cref="ITestSink"/> which is capturing the logs for the given <see cref="WebApplicationFactory{TStartup}"/>.
        /// </summary>
        /// <typeparam name="TStartup">The type of the startup class.</typeparam>
        /// <param name="factory">The <see cref="WebApplicationFactory{TStartup}"/> used in the current test.</param>
        /// <param name="testSink">The <see cref="ITestSink"/> which is capturing logs, if configured.</param>
        /// <returns>True if the <see cref="WebApplicationFactory{TStartup}"/> has been configured to use the test logger.</returns>
        [Obsolete("The recommended alternative is " + nameof(TryGetTestLoggerSink) + "(out " + nameof(ITestLoggerSink) + ")")]
        public static bool TryGetTestSink<TStartup>(this WebApplicationFactory<TStartup> factory, out ITestSink? testSink)
            where TStartup : class
        {
            if (TryGetServices(factory, out var services) && services != null)
            {
                testSink = services.GetService<ITestSink>();
                return testSink != null;
            }

            testSink = null;
            return false;
        }

        /// <summary>
        /// Gets the <see cref="ITestSink"/> which is capturing the logs for the given <see cref="WebApplicationFactory{TStartup}"/>.
        /// </summary>
        /// <typeparam name="TStartup">The type of the startup class.</typeparam>
        /// <param name="factory">The <see cref="WebApplicationFactory{TStartup}"/> used in the current test.</param>
        /// <returns>The <see cref="ITestSink"/> which is capturing logs.</returns>
        [Obsolete("The recommended alternative is " + nameof(GetTestLoggerSink) + "()")]
        public static ITestSink GetTestSink<TStartup>(this WebApplicationFactory<TStartup> factory)
            where TStartup : class
            => GetServices(factory).GetRequiredService<ITestSink>();

        /// <summary>
        /// Tries to get the <see cref="ITestLoggerSink"/> which is capturing the logs for the given <see cref="WebApplicationFactory{TStartup}"/>.
        /// </summary>
        /// <typeparam name="TStartup">The type of the <see cref="WebApplicationFactory{TStartup}"/>.</typeparam>
        /// <param name="factory">The <see cref="WebApplicationFactory{TStartup}"/> used in the current test.</param>
        /// <param name="loggerSink">The <see cref="ITestLoggerSink"/> which is capturing logs, if configured.</param>
        /// <returns>True if the the <see cref="WebApplicationFactory{TStartup}"/> has been configured to use the test logger using builder.UseSerilogTestLogging()
        /// or builder.ConfigureLogging(logging => logging.AddSerilogTest()).</returns>
        public static bool TryGetTestLoggerSink<TStartup>(this WebApplicationFactory<TStartup> factory, out ITestLoggerSink? loggerSink)
            where TStartup : class
        {
            if (TryGetServices(factory, out var services) && services != null)
            {
                loggerSink = services.GetService<ITestLoggerSink>();
                return loggerSink != null;
            }

            loggerSink = null;
            return false;
        }

        /// <summary>
        /// Gets the <see cref="ITestLoggerSink"/> which is capturing the logs for the given <see cref="WebApplicationFactory{TStartup}"/>.
        /// </summary>
        /// <typeparam name="TStartup">The type of the <see cref="WebApplicationFactory{TStartup}"/>.</typeparam>
        /// <param name="factory">The <see cref="WebApplicationFactory{TStartup}"/> used in the current test.</param>
        /// <returns>The <see cref="ITestLoggerSink"/> which is capturing logs.</returns>
        /// <exception cref="InvalidOperationException">
        /// Thrown when the <see cref="WebApplicationFactory{TStartup}"/> has not been configured to use the test logger using builder.UseTestLogging()
        /// or builder.ConfigureLogging(logging => logging.AddTest()).
        /// </exception>
        public static ITestLoggerSink GetTestLoggerSink<TStartup>(this WebApplicationFactory<TStartup> factory)
            where TStartup : class
            => GetServices(factory).GetRequiredService<ITestLoggerSink>();


        internal static IServiceProvider GetServices<TStartup>(WebApplicationFactory<TStartup> factory)
            where TStartup : class
            => factory.Services;

        internal static bool TryGetServices<TStartup>(WebApplicationFactory<TStartup> factory, out IServiceProvider? services)
            where TStartup : class
        {
            services = factory.Services;
            return true;
        }
    }
}
