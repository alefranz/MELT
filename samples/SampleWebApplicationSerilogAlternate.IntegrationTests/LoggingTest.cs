using MELT;
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
        private readonly ISerilogTestSink _sink;
        private readonly WebApplicationFactory<Startup> _factory;

        public LoggingTest(WebApplicationFactory<Startup> factory)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                .Enrich.FromLogContext()
                .WriteTo.Providers(Program.Providers)
                .CreateLogger();

            //var sink = MELTBuilder.CreateTestSink(options => options.FilterByNamespace(nameof(SampleWebApplicationSerilogAlternate)));
            //_factory = factory.WithWebHostBuilder(builder => builder.ConfigureLogging(logging => logging.AddTestLogger(_sink)));
            //_sink = sink.AsSerilog();

            _factory = factory.WithWebHostBuilder(builder => builder.UseTestLogging(options => options.FilterByNamespace(nameof(SampleWebApplicationSerilogAlternate))));
            _sink = _factory.GetTestLoggerSink().AsSerilog();
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
            var scope = Assert.Single(log.GetSerilogScope());
            var scopeValue = Assert.IsType<ScalarValue>(scope).Value;
            Assert.Equal("I'm in the GET scope", scopeValue);
        }

        [Fact]
        public async Task ShouldUseNestedScope()
        {
            // Arrange

            // Act
            await _factory.CreateDefaultClient().GetAsync("/nestedScope");

            // Assert
            var log = Assert.Single(_sink.LogEntries);

            // Assert the scopes
            var scopeElements = log.GetSerilogScope();
            Assert.Equal(2, scopeElements.Count);
            Assert.Equal(new ScalarValue("A top level scope"), scopeElements[0]);
            Assert.Equal(new ScalarValue("I'm in the GET scope"), scopeElements[1]);
            // or
            Assert.Collection(scopeElements,
                x => Assert.Equal(new ScalarValue("A top level scope"), x),
                x => Assert.Equal(new ScalarValue("I'm in the GET scope"), x)
            );
        }

        [Fact]
        public async Task ShouldUseScopeWithParameter()
        {
            // Arrange

            // Act
            await _factory.CreateDefaultClient().GetAsync("/");

            // Assert
            var log = Assert.Single(_sink.LogEntries);
            // Assert specific parameters in the log entry itself, as Serilog puts the scope parameters on the log entry
            LogValuesAssert.Contains("name", "GET", log);
        }

        [Fact]
        public async Task ShouldHaveDestructuredObjectInLogMessage()
        {
            // Arrange

            // Act
            await _factory.CreateDefaultClient().GetAsync("/destructure");

            // Assert
            var log = Assert.Single(_sink.LogEntries);
            // Assert the message rendered by a default formatter
            Assert.Equal("This { foo: \"bar\", answer: 42 } has been destructured.", log.Message);
        }

        [Fact]
        public async Task ShouldHaveDestructuredProperty()
        {
            // Arrange

            // Act
            await _factory.CreateDefaultClient().GetAsync("/destructure");

            // Assert
            var log = Assert.Single(_sink.LogEntries);
            // Assert specific parameters in the log entry
            //StructureValue expected = new StructureValue(new[] {
            //    new LogEventProperty("foo", new ScalarValue("bar")),
            //    new LogEventProperty("answer", new ScalarValue(42)),
            //});
            //LogValuesAssert.Contains("thing", expected, log);

            // or
            //LogValuesAssert.Contains("thing", "{ foo: \"bar\", answer: 42 }", log);

            var thing = Assert.Single(log.Properties, x => x.Key == "thing");
            var value = Assert.IsType<StructureValue>(thing.Value);
            var foo = Assert.Single(value.Properties, x => x.Name == "foo");
            Assert.Equal(new ScalarValue("bar"), foo.Value);
            var answer = Assert.Single(value.Properties, x => x.Name == "answer");
            Assert.Equal(new ScalarValue(42), answer.Value);
        }

        public void Dispose()
        {
            Log.CloseAndFlush();
        }
    }
}
