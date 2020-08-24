using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace SampleWebApplication2_1.IntegrationTests
{
    public class LoggingTestWithInjectedFactory : IClassFixture<CustomWebApplicationFactory<Startup>>
    {
        private readonly CustomWebApplicationFactory<Startup> _factory;

        public LoggingTestWithInjectedFactory(CustomWebApplicationFactory<Startup> factory)
        {
            _factory = factory;
            // In this case, the factory will be reused for all tests, so the sink will be shared as well.
            // We can clear the sink before each test execution, as xUnit will not run this tests in parallel.
            if (_factory.TryGetTestSink(out var testSink)) testSink!.Clear();  // or simply testSink.Clear(); when not using Nullable Reference Types
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
    }

    public class CustomWebApplicationFactory<TStartup> : WebApplicationFactory<TStartup>
         where TStartup : class
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseTestLogging(options => options.FilterByNamespace(nameof(SampleWebApplication2_1)));
        }
    }
}
