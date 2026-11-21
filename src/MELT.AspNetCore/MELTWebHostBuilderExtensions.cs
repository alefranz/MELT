using System;
using MELT;
using Microsoft.Extensions.Logging;

namespace Microsoft.AspNetCore.Hosting
{
    /// <summary>
    /// Extension methods for <see cref="IWebHostBuilder"/> to add test logging capabilities.
    /// </summary>
    public static class MELTWebHostBuilderExtensions
    {
        /// <summary>
        /// Adds a default test logger that collect logs.
        /// </summary>
        /// <remarks>
        /// Repeated calls replace the previous MELT test logger provider and sink registration so that the latest configuration takes effect.
        /// </remarks>
        /// <param name="builder">The <see cref="IWebHostBuilder"/> to add the test logger to.</param>
        /// <returns>A reference to the <paramref name="builder"/> after the operation has completed.</returns>
        public static IWebHostBuilder UseTestLogging(this IWebHostBuilder builder)
            => builder.ConfigureLogging(logging => logging.AddTest());

        /// <summary>
        /// Adds a configured test logger that collect logs.
        /// </summary>
        /// <remarks>
        /// Repeated calls replace the previous MELT test logger provider and sink registration so that the latest configuration takes effect.
        /// </remarks>
        /// <param name="builder">The <see cref="IWebHostBuilder"/> to add the test logger to.</param>
        /// <param name="configure">A delegate used to configure the <see cref="TestLoggerOptions"/>.</param>
        /// <returns>A reference to the <paramref name="builder"/> after the operation has completed.</returns>
        public static IWebHostBuilder UseTestLogging(this IWebHostBuilder builder, Action<TestLoggerOptions> configure)
            => builder.ConfigureLogging(logging => logging.AddTest(configure));
    }
}
