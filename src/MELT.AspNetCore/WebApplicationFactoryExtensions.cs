using MELT;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace SampleWebApplication.Tests
{
    public static class WebApplicationFactoryExtensions
    {
        public static ITestSink GetTestSink<TStartup>(this WebApplicationFactory<TStartup> factory)
            where TStartup : class
            => GetServices(factory).GetRequiredService<ITestSink>();


        private static IServiceProvider GetServices<TStartup>(WebApplicationFactory<TStartup> factory) where TStartup : class
        {
            IWebHost host;
            try
            {
                host = factory.Server.Host;
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
    }
}
