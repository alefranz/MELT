using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Logging;
using Xunit;

namespace SampleWebApplication3_1.IntegrationTests
{
    public class LoggingTestAlternate : IClassFixture<WebApplicationFactory<Startup>>
    {
        private readonly WebApplicationFactory<Startup> _factory;

        public LoggingTestAlternate(WebApplicationFactory<Startup> factory)
        {
            _factory = factory.WithWebHostBuilder(builder =>
                builder.ConfigureLogging(logging =>
                    logging.AddTest(options =>
                        options.FilterByNamespace(nameof(SampleWebApplication3_1)))));
        }

        [Fact]
        public async Task ShouldLogHelloWorld()
        {
            // Arrange

            // Act
            await _factory.CreateDefaultClient().GetAsync("/");

            // Assert
            var log = Assert.Single(_factory.GetTestLoggerSink().LogEntries);
            // Assert the message rendered by a default formatter
            Assert.Equal("Hello World!", log.Message);
        }
    }
}
