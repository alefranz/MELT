using MELT;
using MELT.Xunit;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using System;
using System.Collections.Generic;
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
            Assert.Equal("[\"A top level scope\", \"I'm in the GET scope\"]", log.Scope.Message);

            // or Assert the scope raw property
            var scopeSequence = Assert.IsType<SequenceValue>(log.Properties.Single(x => x.Key == "Scope").Value);
            Assert.Equal(2, scopeSequence.Elements.Count);
            Assert.Equal("\"A top level scope\"", scopeSequence.Elements[0].ToString());
            Assert.Equal("\"I'm in the GET scope\"", scopeSequence.Elements[1].ToString());
            // or
            Assert.Collection(scopeSequence.Elements,
                x => Assert.Equal("\"A top level scope\"", x.ToString()),
                x => Assert.Equal("\"I'm in the GET scope\"", x.ToString())
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
            Assert.Equal("\"I'm in the GET scope\"", scope.Message);
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
