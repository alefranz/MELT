using MELT;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Microsoft.AspNetCore.Mvc.Testing
{
    public static class MELTWebApplicationFactoryExtensions
    {
        [Obsolete("The recommended alternative is " + nameof(TryGetTestLoggerSink) + "(out " + nameof(ITestLoggerSink) +")")]
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

        [Obsolete("The recommended alternative is " + nameof(GetTestLoggerSink) + "()")]
        public static ITestSink GetTestSink<TStartup>(this WebApplicationFactory<TStartup> factory)
            where TStartup : class
            => GetServices(factory).GetRequiredService<ITestSink>();

        public static bool TryGetTestLoggerSink<TStartup>(this WebApplicationFactory<TStartup> factory, out ITestLoggerSink? testSink)
            where TStartup : class
        {
            if (TryGetServices(factory, out var services))
            {
                testSink = services.GetService<ITestLoggerSink>();
                return testSink != null;
            }

            testSink = null;
            return false;
        }

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
