using System;
using MELT;

namespace Microsoft.Extensions.Logging
{
    /// <summary>
    /// Extension methods for <see cref="ILoggerFactory"/> to work with test logging capabilities.
    /// </summary>
    public static class MELTLoggerFactoryExtensions
    {
        /// <summary>
        /// Gets the test logger sink from the logger factory.
        /// </summary>
        /// <param name="loggerFactory">The logger factory to get the sink from.</param>
        /// <returns>The <see cref="ITestLoggerSink"/> instance associated with the factory.</returns>
        /// <exception cref="ArgumentException">Thrown when the logger factory is not created with <see cref="TestLoggerFactory"/>.</exception>
        public static ITestLoggerSink GetTestLoggerSink(this ILoggerFactory loggerFactory)
        {
            if (loggerFactory is TestLoggerFactory testLoggerFactory) return testLoggerFactory.Sink;

            throw new ArgumentException($"The {nameof(ILoggerFactory)} has not be created with {nameof(TestLoggerFactory)}.{nameof(TestLoggerFactory.Create)}().", nameof(loggerFactory));
        }
    }
}
