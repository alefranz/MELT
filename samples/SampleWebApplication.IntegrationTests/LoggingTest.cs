using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace SampleWebApplication.IntegrationTests
{
    public class LoggingTest : IClassFixture<WebApplicationFactory<Startup>>
    {
        private readonly WebApplicationFactory<Startup> _factory;

        public LoggingTest(WebApplicationFactory<Startup> factory)
        {
            _factory = factory.WithWebHostBuilder(builder => builder.UseTestLogging(options => options.FilterByNamespace(nameof(SampleWebApplication))));

            // or
            //_factory = factory.WithWebHostBuilder(builder =>
            //    builder.ConfigureLogging(logging =>
            //        logging.AddTest(options =>
            //            options.FilterByNamespace(nameof(SampleWebApplication)))));
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
        public async Task ShouldLogHelloWorldAndUniverse()
        {
            // Arrange  

            // Act
            await _factory.CreateDefaultClient().GetAsync("/?multipleValues=1");

            // Assert
            var log = Assert.Single(_factory.GetTestLoggerSink().LogEntries);
            // Assert the message rendered by a default formatter
            Assert.Equal("Hello World and Universe!", log.Message);
        }

        [Fact]
        public async Task ShouldLogWithMultipleValuesForPlace()
        {
            // Arrange  

            // Act
            await _factory.CreateDefaultClient().GetAsync("/?multipleValues=1");

            // Assert
            var log = Assert.Single(_factory.GetTestLoggerSink().LogEntries);
            // Assert specific parameters in the log entry
            LoggingAssert.Contains("place", "World", log.Properties);
            LoggingAssert.Contains("place", "Universe", log.Properties);
            // or
            LoggingAssert.Contains(new[] {
                new KeyValuePair<string, object> ("place", "World"),
                new KeyValuePair<string, object> ("place", "Universe")
            }, log.Properties);
        }

        [Fact]
        public async Task ShouldUseScope()
        {
            // Arrange  

            // Act
            await _factory.CreateDefaultClient().GetAsync("/");

            // Assert
            var log = Assert.Single(_factory.GetTestLoggerSink().LogEntries);
            // Assert the scope rendered by a default formatter
            Assert.Equal("I'm in the GET scope", log.Scope.Message);
        }

        [Fact]
        public async Task ShouldUseScopeWithParameter()
        {
            // Arrange  

            // Act
            await _factory.CreateDefaultClient().GetAsync("/");

            // Assert
            var log = Assert.Single(_factory.GetTestLoggerSink().LogEntries);
            // Assert specific parameters in the log scope
            LoggingAssert.Contains("name", "GET", log.Scope.Properties);
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
