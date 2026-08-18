using System;
using System.Threading.Tasks;
using MELT.Xunit;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Serilog;
using Serilog.Events;
using Xunit;

namespace SampleWebApplicationSerilog.Tests
{
    [Collection("Serilog Test Collection")]
    public class LoggingTest : IClassFixture<WebApplicationFactory<Startup>>, IDisposable
    {
        private readonly WebApplicationFactory<Startup> _factory;

        public LoggingTest(WebApplicationFactory<Startup> factory)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .Enrich.FromLogContext()
                .WriteTo.Providers(Program.Providers)
                .CreateLogger();

            _factory = factory.WithWebHostBuilder(builder => builder.UseSerilogTestLogging(options => options.FilterByNamespace(nameof(SampleWebApplicationSerilog))));
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
        public async Task ShouldLogWithWorldAsPlace()
        {
            // Arrange

            // Act
            await _factory.CreateDefaultClient().GetAsync("/");

            // Assert
            var log = Assert.Single(_factory.GetSerilogTestLoggerSink().LogEntries);
            // Assert specific parameters in the log entry
            LoggingAssert.Contains("place", "World", log.Properties);
        }

        [Fact]
        public async Task ShouldUseScope()
        {
            // Arrange

            // Act
            await _factory.CreateDefaultClient().GetAsync("/");

            // Assert
            var log = Assert.Single(_factory.GetSerilogTestLoggerSink().LogEntries);
            var scope = Assert.Single(log.Scopes);
            // Assert the scope rendered by a default formatter
            Assert.Equal(new ScalarValue("I'm in the GET scope"), scope);
        }

        [Fact]
        public async Task ShouldUseScopeWithParameter()
        {
            // Arrange

            // Act
            await _factory.CreateDefaultClient().GetAsync("/");

            // Assert
            var log = Assert.Single(_factory.GetSerilogTestLoggerSink().LogEntries);
            // Serilog adds structured scope parameters to the log event.
            LoggingAssert.Contains("name", "GET", log.Properties);
        }

        [Fact]
        public async Task ShouldBeginScope()
        {
            // Arrange

            // Act
            await _factory.CreateDefaultClient().GetAsync("/");

            // Assert
            var log = Assert.Single(_factory.GetSerilogTestLoggerSink().LogEntries);
            var scope = Assert.Single(log.Scopes);
            // Assert the scope rendered by a default formatter
            Assert.Equal(new ScalarValue("I'm in the GET scope"), scope);
        }

        [Fact]
        public async Task ShouldBeginScopeWithParameter()
        {
            // Arrange

            // Act
            await _factory.CreateDefaultClient().GetAsync("/");

            // Assert
            var log = Assert.Single(_factory.GetSerilogTestLoggerSink().LogEntries);
            // Serilog adds structured scope parameters to the log event.
            LoggingAssert.Contains("name", "GET", log.Properties);
        }

        public void Dispose()
        {
            Log.CloseAndFlush();
        }
    }
}
