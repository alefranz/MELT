using MELT;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Microsoft.AspNetCore.Mvc.Testing
{
    /// <summary>
    /// Extensions for <see cref="WebApplicationFactory{TStartup}"/> to use when testing logging.
    /// </summary>
    public static class MELTSerilogWebApplicationFactoryExtensions
    {
        /// <summary>
        /// Gets the <see cref="ISerilogTestLoggerSink"/> which is capturing the logs for the given <see cref="WebApplicationFactory{TStartup}"/>.
        /// </summary>
        /// <typeparam name="TStartup">The type of the <see cref="WebApplicationFactory{TStartup}"/>.</typeparam>
        /// <param name="factory">The <see cref="WebApplicationFactory{TStartup}"/> used in the current test.</param>
        /// <returns>The <see cref="ISerilogTestLoggerSink"/> which is capturing logs.</returns>
        /// <exception cref="InvalidOperationException">
        /// Thrown when the <see cref="WebApplicationFactory{TStartup}"/> has not been configured to use the test logger using builder.UseSerilogTestLogging()
        /// or builder.ConfigureLogging(logging => logging.AddSerilogTest()).
        /// </exception>
        public static ISerilogTestLoggerSink GetSerilogTestLoggerSink<TStartup>(this WebApplicationFactory<TStartup> factory)
            where TStartup : class
            => MELTWebApplicationFactoryExtensions.GetServices(factory).GetRequiredService<ISerilogTestLoggerSink>();

        /// <summary>
        /// Tries to get the <see cref="ISerilogTestLoggerSink"/> which is capturing the logs for the given <see cref="WebApplicationFactory{TStartup}"/>.
        /// </summary>
        /// <typeparam name="TStartup">The type of the <see cref="WebApplicationFactory{TStartup}"/>.</typeparam>
        /// <param name="factory">The <see cref="WebApplicationFactory{TStartup}"/> used in the current test.</param>
        /// <param name="loggerSink">The <see cref="ISerilogTestLoggerSink"/> which is capturing logs, if configured.</param>
        /// <returns>True if the the <see cref="WebApplicationFactory{TStartup}"/> has been configured to use the test logger using builder.UseSerilogTestLogging()
        /// or builder.ConfigureLogging(logging => logging.AddSerilogTest()).</returns>
        public static bool TryGetTestLoggerSink<TStartup>(this WebApplicationFactory<TStartup> factory, out ISerilogTestLoggerSink? loggerSink)
            where TStartup : class
        {
            if (MELTWebApplicationFactoryExtensions.TryGetServices(factory, out var services))
            {
                loggerSink = services.GetService<ISerilogTestLoggerSink>();
                return loggerSink != null;
            }

            loggerSink = null;
            return false;
        }
    }
}
