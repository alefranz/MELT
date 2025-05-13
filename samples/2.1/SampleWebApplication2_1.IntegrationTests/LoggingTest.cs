using System;
using System.Threading.Tasks;
using MELT;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net.Http;
using Xunit;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.TestHost;

namespace SampleWebApplication2_1.IntegrationTests
{
    public class LoggingTest : IClassFixture<WebApplicationFactory<Startup>>
    {
        private readonly WebApplicationFactory<Startup> _factory;

        public LoggingTest(WebApplicationFactory<Startup> factory)
        {
            _factory = factory.WithWebHostBuilder(
                builder => builder
                    .UseSolutionRelativeContentRoot(Environment.CurrentDirectory)
                    .UseTestLogging(options => options.FilterByNamespace(nameof(SampleWebApplication2_1))));
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

        [Fact]
        public async Task ShouldLogWithWorldAsPlace()
        {
            // Arrange

            // Act
            await _factory.CreateDefaultClient().GetAsync("/");

            // Assert
            var log = Assert.Single(_factory.GetTestLoggerSink().LogEntries);
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
            var log = Assert.Single(_factory.GetTestLoggerSink().LogEntries);
            var scope = Assert.Single(log.Scopes);
            // Assert the scope rendered by a default formatter
            Assert.Equal("I'm in the GET scope", scope.Message);
        }

        [Fact]
        public async Task ShouldUseScopeWithParameter()
        {
            // Arrange

            // Act
            await _factory.CreateDefaultClient().GetAsync("/");

            // Assert
            var log = Assert.Single(_factory.GetTestLoggerSink().LogEntries);
            var scope = Assert.Single(log.Scopes);
            // Assert specific parameters in the log scope
            LoggingAssert.Contains("name", "GET", scope.Properties);
        }

        [Fact]
        public async Task ShouldBeginScope()
        {
            // Arrange

            // Act
            await _factory.CreateDefaultClient().GetAsync("/");

            // Assert
            var scope = Assert.Single(_factory.GetTestLoggerSink().Scopes);
            // Assert the scope rendered by a default formatter
            Assert.Equal("I'm in the GET scope", scope.Message);
        }

        [Fact]
        public async Task ShouldBeginScopeWithParameter()
        {
            // Arrange

            // Act
            await _factory.CreateDefaultClient().GetAsync("/");

            // Assert
            var scope = Assert.Single(_factory.GetTestLoggerSink().Scopes);
            // Assert specific parameters in the log scope
            LoggingAssert.Contains("name", "GET", scope.Properties);
        }
    }
}
