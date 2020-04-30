using MELT;
using MELT.Xunit;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace SampleWebApplicationSerilogAlternate.Tests
{
    [Collection("Serilog Test Collection")]
    public class LoggingTest : IClassFixture<WebApplicationFactory<Startup>>, IDisposable
    {
        private readonly ITestSink _sink;
        private readonly WebApplicationFactory<Startup> _factory;

        public LoggingTest(WebApplicationFactory<Startup> factory)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                .Enrich.FromLogContext()
                .WriteTo.Providers(Program.Providers)
                .CreateLogger();

            _sink = MELTBuilder.CreateTestSink(options => options.FilterByNamespace(nameof(SampleWebApplicationSerilogAlternate)));
            _factory = factory.WithWebHostBuilder(builder => builder.ConfigureLogging(logging => logging.AddTestLogger(_sink, useScopeFromProperties: true)));
        }

        [Fact]
        public async Task ShouldLogHelloWorld()
        {
            // Arrange

            // Act
            await _factory.CreateDefaultClient().GetAsync("/");

            var log = Assert.Single(_sink.LogEntries);
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
            var log = Assert.Single(_sink.LogEntries);
            // Assert specific parameters in the log entry
            LogValuesAssert.Contains("place", "World", log);
        }

        [Fact]
        public async Task ShouldUseScope()
        {
            // Arrange

            // Act
            await _factory.CreateDefaultClient().GetAsync("/");

            // Assert
            var log = Assert.Single(_sink.LogEntries);
            // Assert the scope rendered by a default formatter
            //Assert.Equal("I'm in the GET scope", log.Scope.Message);
            var scopeSequence = Assert.IsType<SequenceValue>(log.Properties.Single(x => x.Key == "Scope").Value);
            var scope = Assert.Single(scopeSequence.Elements);
            Assert.Equal("\"I'm in the GET scope\"", scope.ToString());
        }

        [Fact]
        public async Task ShouldUseNestedScope()
        {
            // Arrange

            // Act
            await _factory.CreateDefaultClient().GetAsync("/nestedScope");

            // Assert
            var log = Assert.Single(_sink.LogEntries);
            // Assert the scope rendered by a default formatter
            //Assert.Equal("I'm in the GET scope", log.Scope.Message);
            var scopeSequence = Assert.IsType<SequenceValue>(log.Properties.Single(x => x.Key == "Scope").Value);
            Assert.Equal(2, scopeSequence.Elements.Count);
            Assert.Equal("\"A top level scope\"", scopeSequence.Elements[0].ToString());
            Assert.Equal("\"I'm in the GET scope\"", scopeSequence.Elements[1].ToString());

            // TODO: create extension method for this check: GetSerilogScope() that get Elements
            // remove the bool in the logger to have the logic in the logger
        }

        [Fact]
        public async Task ShouldUseScopeWithParameter()
        {
            // Arrange

            // Act
            await _factory.CreateDefaultClient().GetAsync("/");

            // Assert
            var log = Assert.Single(_sink.LogEntries);
            // Assert specific parameters in the log scope
            LogValuesAssert.Contains("name", "GET", log.Scope);
        }

        [Fact]
        public async Task ShouldBeginScope()
        {
            // Arrange

            // Act
            await _factory.CreateDefaultClient().GetAsync("/");

            // Assert
            var scope = Assert.Single(_sink.Scopes);
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
            var scope = Assert.Single(_sink.Scopes);
            // Assert specific parameters in the log scope
            LogValuesAssert.Contains("name", "GET", scope);
        }

        [Fact]
        public async Task ShouldDestructure()
        {
            // Arrange

            // Act
            await _factory.CreateDefaultClient().GetAsync("/destructure");

            var log = Assert.Single(_sink.LogEntries);
            // Assert the message rendered by a default formatter
            Assert.Equal("This { foo: \"bar\", answer: 42 } has been destructured.", log.Message);
        }

        public void Dispose()
        {
            Log.CloseAndFlush();
        }
    }
}
