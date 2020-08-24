using System;
using MELT;
using Microsoft.Extensions.Logging;

namespace Microsoft.AspNetCore.Hosting
{
    public static class MELTSerilogWebHostBuilderExtensions
    {
        /// <summary>
        /// Adds a default test logger, based on Serilog, that collect logs.
        /// </summary>
        /// <param name="builder">The <see cref="IWebHostBuilder"/> to add the test logger to.</param>
        /// <returns>A reference to the <paramref name="builder"/> after the operation has completed.</returns>
        public static IWebHostBuilder UseSerilogTestLogging(this IWebHostBuilder builder)
            => builder.ConfigureLogging(logging => logging.AddSerilogTest());

        /// <summary>
        /// Adds a configured test logger, based on Serilog, that collect logs.
        /// </summary>
        /// <param name="builder">The <see cref="IWebHostBuilder"/> to add the test logger to.</param>
        /// <param name="configure">A delegate used to configure the <see cref="TestLoggerOptions"/>.</param>
        /// <returns>A reference to the <paramref name="builder"/> after the operation has completed.</returns>
        public static IWebHostBuilder UseSerilogTestLogging(this IWebHostBuilder builder, Action<TestLoggerOptions> configure)
            => builder.ConfigureLogging(logging => logging.AddSerilogTest(configure));
    }
}
