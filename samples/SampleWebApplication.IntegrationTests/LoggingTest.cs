using MELT;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace SampleWebApplication.Tests
{
    public class LoggingTest : IClassFixture<WebApplicationFactory<Startup>>
    {
        private readonly ITestSink _sink;
        private readonly WebApplicationFactory<Startup> _factory;

        public LoggingTest(WebApplicationFactory<Startup> factory)
        {
            // Creates a sink that capture only log entry generated in our namespace
            _sink = new TestSink(x => x.LoggerName.StartsWith($"{nameof(SampleWebApplication)}."));

            // Wires the TestSink in the TestHost
            _factory = factory.WithWebHostBuilder(builder => builder.ConfigureLogging(logging => logging.AddProvider(new TestLoggerProvider(_sink))));
        }

        [Fact]
        public async Task ShouldLogHelloWorld()
        {
            // Arrange  

            // Act
            await _factory.CreateDefaultClient().GetAsync("/");

            // Assert
            Assert.Equal(1, _sink.Writes.Count);
            var log = _sink.Writes.Single();
            Assert.Equal("Hello World!", log.Message); // Assert the message rendered by a default formatter
        }
    }
}
