using MELT;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.AspNetCore.Mvc.Testing
{
    public static class MELTSerilogWebApplicationFactoryExtensions
    {
        public static ISerilogTestLoggerSink GetSerilogTestLoggerSink<TStartup>(this WebApplicationFactory<TStartup> factory)
            where TStartup : class
            => MELTWebApplicationFactoryExtensions.GetServices(factory).GetRequiredService<ISerilogTestLoggerSink>();
    }
}
