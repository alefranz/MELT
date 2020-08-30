using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Serilog;

namespace SampleWebApplicationSerilogAlternate.IntegrationTests
{
    public class CustomWebApplicationFactory<TStartup> : WebApplicationFactory<TStartup>
         where TStartup : class
    {
        public CustomWebApplicationFactory()
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .Enrich.FromLogContext()
                .WriteTo.Providers(Program.Providers)
                .CreateLogger();
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseSerilogTestLogging(options =>
            {
                options.FilterByNamespace(nameof(SampleWebApplicationSerilogAlternate));
            });
        }

        protected override void Dispose(bool disposing)
        {
            if (true)
            {
                Log.CloseAndFlush();
            }
        }
    }
}
