using MELT;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Microsoft.AspNetCore.Hosting
{
    public static class WebHostBuilderExtensions
    {
        public static IWebHostBuilder UseTestLogging(this IWebHostBuilder builder)
        {
            return builder.ConfigureLogging(logging =>
            {
                var sink = MELTBuilder.CreateTestSink();
                logging.Services.AddSingleton(sink);
                logging.AddTestLogger(sink);
            });
        }
    }
}
