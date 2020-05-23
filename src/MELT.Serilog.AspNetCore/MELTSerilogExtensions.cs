using MELT.Serilog;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.AspNetCore.Mvc.Testing
{
    public static class MELTSerilogExtensions
    {
        //public static ISerilogTestLoggerSink AsSerilog(this ITestLoggerSink sink)
        //{
        //    return new SerilogTestLoggerSink(sink);
        //}

        public static ISerilogTestLoggerSink GetSerilogTestLoggerSink<TStartup>(this WebApplicationFactory<TStartup> factory)
            where TStartup : class
            => MELTWebApplicationFactoryExtensions.GetServices(factory).GetRequiredService<ISerilogTestLoggerSink>();
    }
}
