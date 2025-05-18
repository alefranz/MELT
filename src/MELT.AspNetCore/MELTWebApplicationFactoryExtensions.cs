using System;
using MELT;
using Microsoft.AspNetCore.Hosting;
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
            if (TryGetServices(factory, out var services))
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
            if (TryGetServices(factory, out var services))
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


        internal static IServiceProvider GetServices<TStartup>(WebApplicationFactory<TStartup> factory) where TStartup : class
        {
            var server = factory.Server;

            if (server == null)
            {
                var message =
                    $"When running on 2.x, the server is not initialized until it is explicitly started or the first client is created. " +
                    $"Consider using '{nameof(TryGetTestSink)}()' instead.";
                throw new InvalidOperationException(message);
            }

            IWebHost host;
            try
            {
                host = server.Host;
            }
            catch (InvalidOperationException)
            {
                // We are probably running on 3.0 with generic host
                // but we are referencing a lower version of the package here
                // so try to retrieve the Services with reflection
                if (factory.GetType().GetProperty("Services")?.GetValue(factory, null) is IServiceProvider services) return services;

                // It looks like, after all, we are not running on 3.0
                throw;
            }

            return host.Services;
        }

        internal static bool TryGetServices<TStartup>(WebApplicationFactory<TStartup> factory, out IServiceProvider? services) where TStartup : class
        {
            IWebHost? host = null;
            try
            {
                host = factory.Server?.Host;
            }
            catch (InvalidOperationException)
            {
                // We are probably running on 3.0 with generic host
                // but we are referencing a lower version of the package here
                // so try to retrieve the Services with reflection
                if (factory.GetType().GetProperty("Services")?.GetValue(factory, null) is IServiceProvider serviceProvider)
                {
                    services = serviceProvider;
                    return true;
                }
            }

            services = host?.Services;

            return services != null ? true : false;
        }
    }
}
