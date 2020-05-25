using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Serilog;
using Serilog.Events;
using System.Threading.Tasks;
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
            // In this case the factory will be resused for all tests, so the sink will be shared as well.
            // We can clear the sink before each test execution, as xUnit will not run this tests in parallel.
            _factory.GetSerilogTestLoggerSink().Clear();
            // When running on 2.x, the server is not initialized until it is explicitly started or the first client is created.
            // So we need to use:
            // if (_factory.TryGetSerilogTestLoggerSink(out var testSink)) testSink!.Clear();
            // The exclamation mark is needed only when using Nullable Reference Types!
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
