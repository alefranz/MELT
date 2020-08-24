using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Serilog.Events;
using Xunit;

namespace SampleWebApplicationSerilogAlternate.IntegrationTests
{
    [Collection("Serilog Test Collection")]
    public class LoggingTestWithInjectedFactory : IClassFixture<CustomWebApplicationFactory<Startup>>
    {
        private readonly CustomWebApplicationFactory<Startup> _factory;

        public LoggingTestWithInjectedFactory(CustomWebApplicationFactory<Startup> factory)
        {
            _factory = factory;
            // In this case, the factory will be reused for all tests, so the sink will be shared as well.
            // We can clear the sink before each test execution, as xUnit will not run this tests in parallel.
            _factory.GetSerilogTestLoggerSink().Clear();
            // When running on 2.x, the server is not initialized until it is explicitly started or the first client is created.
            // So we need to use:
            // if (_factory.TryGetSerilogTestLoggerSink(out var testLoggerSink)) testLoggerSink.Clear();
        }

        [Fact]
        public async Task ShouldLogHelloWorld()
        {
            // Arrange

            // Act
            await _factory.CreateDefaultClient().GetAsync("/");

            // Assert
            var log = Assert.Single(_factory.GetSerilogTestLoggerSink().LogEntries);
            // Assert the message rendered by a default formatter
            Assert.Equal("Hello \"World\"!", log.Message);
        }

        [Fact]
        public async Task ShouldUseScope()
        {
            // Arrange

            // Act
            await _factory.CreateDefaultClient().GetAsync("/");

            // Assert
            var log = Assert.Single(_factory.GetSerilogTestLoggerSink().LogEntries);
            var scope = Assert.Single(log.Scope);
            var scopeValue = Assert.IsType<ScalarValue>(scope).Value;
            Assert.Equal("I'm in the GET scope", scopeValue);
        }
    }
}
