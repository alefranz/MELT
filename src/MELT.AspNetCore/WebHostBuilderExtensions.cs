using MELT;
using Microsoft.Extensions.Logging;
using System;

namespace Microsoft.AspNetCore.Hosting
{
    public static class WebHostBuilderExtensions
    {
        /// <summary>
        /// Adds a default test logger that collect logs.
        /// </summary>
        /// <param name="builder">The <see cref="IWebHostBuilder"/> to add the test logger to.</param>
        /// <returns>A reference to the <paramref name="builder"/> after the operation has completed.</returns>
        public static IWebHostBuilder UseTestLogging(this IWebHostBuilder builder)
            => builder.ConfigureLogging(logging => logging.AddTestLogger());

        /// <summary>
        /// Adds a configured test logger that collect logs.
        /// </summary>
        /// <param name="builder">The <see cref="IWebHostBuilder"/> to add the test logger to.</param>
        /// <param name="configure">A delegate used to configure the <see cref="TestSinkOptions"/>.</param>
        /// <returns>A reference to the <paramref name="builder"/> after the operation has completed.</returns>
        public static IWebHostBuilder UseTestLogging(this IWebHostBuilder builder, Action<TestSinkOptions> configure)
            => builder.ConfigureLogging(logging => logging.AddTestLogger(configure));

        /// <summary>
        /// Adds a configured test logger that collect logs.
        /// </summary>
        /// <param name="builder">The <see cref="IWebHostBuilder"/> to add the test logger to.</param>
        /// <param name="sink">The <see cref="ITestSink"/> to send the logs to.</param>
        /// <returns>A reference to the <paramref name="builder"/> after the operation has completed.</returns>
        public static IWebHostBuilder UseTestLogging(this IWebHostBuilder builder, ITestSink sink)
            => builder.ConfigureLogging(logging => logging.AddTestLogger(sink));
    }
}
