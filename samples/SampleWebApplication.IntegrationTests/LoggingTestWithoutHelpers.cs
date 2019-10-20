using MELT;
using MELT.Xunit;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace SampleWebApplication.Tests
{
    public class LoggingTestWithoutHelpers : IClassFixture<WebApplicationFactory<Startup>>
    {
        private readonly ITestSink _sink;
        private readonly WebApplicationFactory<Startup> _factory;

        public LoggingTestWithoutHelpers(WebApplicationFactory<Startup> factory)
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
            Assert.Equal(1, _sink.Writes.Count);
            var log = _sink.Writes.Single();
            var state = Assert.IsAssignableFrom<IEnumerable<KeyValuePair<string, object>>>(log.State);
            // Assert specific parameters in the log entry
            LogValuesAssert.Contains("place", "World", state);
        }

        [Fact]
        public async Task ShouldUseScope()
        {
            // Arrange  

            // Act
            await _factory.CreateDefaultClient().GetAsync("/");

            // Assert
            Assert.Equal(1, _sink.Writes.Count);
            var log = _sink.Writes.Single();
            // Assert the scope rendered by a default formatter
            Assert.Equal("I'm in the GET scope", log.Scope.ToString());
        }

        [Fact]
        public async Task ShouldUseScopeWithParameter()
        {
            // Arrange  

            // Act
            await _factory.CreateDefaultClient().GetAsync("/");

            // Assert
            Assert.Equal(1, _sink.Writes.Count);
            var log = _sink.Writes.Single();
            var scope = Assert.IsAssignableFrom<IEnumerable<KeyValuePair<string, object>>>(log.Scope);
            // Assert specific parameters in the log scope
            LogValuesAssert.Contains("name", "GET", scope);
        }
    }
}
