using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Xunit;

namespace SampleWebApplication.Tests
{
    public class LoggingTestWithInjectedFactory : IClassFixture<CustomWebApplicationFactory<Startup>>
    {
        private readonly CustomWebApplicationFactory<Startup> _factory;

        public LoggingTestWithInjectedFactory(CustomWebApplicationFactory<Startup> factory)
        {
            _factory = factory;
            // In this case the factory will be resused for all tests, so the sink will be shared as well.
            // We can clear the sink before each test execution, as xUnit will not run this tests in parallel.
            _factory.GetTestSink().Clear();

            if (_factory.TryGetTestSink(out var testSink)) testSink.Clear();
        }

        [Fact]
        public async Task ShouldLogHelloWorld()
        {
            // Arrange  

            // Act
            await _factory.CreateDefaultClient().GetAsync("/");

            // Assert
            var log = Assert.Single(_factory.GetTestSink().LogEntries);
            // Assert the message rendered by a default formatter
            Assert.Equal("Hello World!", log.Message);
        }

        [Fact]
        public async Task ShouldUseScope()
        {
            // Arrange  

            // Act
            await _factory.CreateDefaultClient().GetAsync("/");

            // Assert
            var log = Assert.Single(_factory.GetTestSink().LogEntries);
            // Assert the scope rendered by a default formatter
            Assert.Equal("I'm in the GET scope", log.Scope.Message);
        }

        [Fact]
        public async Task ShouldBeginScope()
        {
            // Arrange  

            // Act
            await _factory.CreateDefaultClient().GetAsync("/");

            // Assert
            var scope = Assert.Single(_factory.GetTestSink().Scopes);
            // Assert the scope rendered by a default formatter
            Assert.Equal("I'm in the GET scope", scope.Message);
        }
    }

    public class CustomWebApplicationFactory<TStartup> : WebApplicationFactory<TStartup>
         where TStartup : class
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureLogging(logging => logging.AddTestLogger(options => options.FilterByNamespace(nameof(SampleWebApplication))));
        }
    }
}
