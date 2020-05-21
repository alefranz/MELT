using Microsoft.AspNetCore.Mvc.Testing;

namespace MELT
{
    public static class SerilogExtensions
    {
        public static ISerilogTestLoggerSink AsSerilog(this ITestLoggerSink sink)
        {
            return new SerilogTestLoggerSink(sink);
        }

        public static ITestLoggerSink GetSerilogTestLoggerSink<TStartup>(this WebApplicationFactory<TStartup> factory)
            where TStartup : class
            => GetServices(factory).GetRequiredService<ITestLoggerSink>();
    }
}
