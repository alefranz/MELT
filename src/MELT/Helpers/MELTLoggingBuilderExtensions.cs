using System;
using MELT;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.Extensions.Logging
{
    /// <summary>
    /// Provides extension methods for adding MELT test logging interceptors to an <see cref="ILoggingBuilder"/>.
    /// </summary>
    public static class MELTLoggingBuilderExtensions
    {
        /// <summary>
        /// Adds a test logger provider to the specified <see cref="ILoggingBuilder"/> with default options.
        /// </summary>
        /// <param name="builder">The <see cref="ILoggingBuilder"/> to add the provider to.</param>
        /// <returns>The <see cref="ILoggingBuilder"/> for chaining.</returns>
        public static ILoggingBuilder AddTest(this ILoggingBuilder builder)
            => builder.AddTestLoggerProvider(new TestLoggerOptions());

        /// <summary>
        /// Adds a test logger provider to the specified <see cref="ILoggingBuilder"/> with custom configuration options.
        /// </summary>
        /// <param name="builder">The <see cref="ILoggingBuilder"/> to add the provider to.</param>
        /// <param name="configure">An action to configure the <see cref="TestLoggerOptions"/>.</param>
        /// <returns>The <see cref="ILoggingBuilder"/> for chaining.</returns>
        public static ILoggingBuilder AddTest(this ILoggingBuilder builder, Action<TestLoggerOptions> configure)
        {
            if (configure == null) throw new ArgumentNullException(nameof(configure));

            var options = TestLoggerOptions.Create(configure);
            return builder.AddTestLoggerProvider(options);
        }

        internal static ILoggingBuilder AddTestLoggerProvider(this ILoggingBuilder builder, TestLoggerOptions options)
        {
            var sink = new TestLoggerSink(options);
            var provider = new TestLoggerProvider(sink);

            for (var i = builder.Services.Count - 1; i >= 0; i--)
            {
                if (builder.Services[i].ServiceType == typeof(ILoggerProvider)
                    && builder.Services[i].ImplementationInstance is TestLoggerProvider)
                {
                    builder.Services.RemoveAt(i);
                }
            }

            builder.Services.Replace(ServiceDescriptor.Singleton(sink));
            builder.Services.Replace(ServiceDescriptor.Singleton<ITestLoggerSink>(sink));
            return builder.AddProvider(provider);
        }
    }
}
