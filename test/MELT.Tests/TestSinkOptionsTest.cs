using System;
using A.B;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xunit;

namespace MELT.Tests
{
    public class TestSinkOptionsTest
    {
        [Theory]
        [InlineData(LogLevel.Information, LogLevel.Debug, false)]
        [InlineData(LogLevel.Information, LogLevel.Information, true)]
        [InlineData(LogLevel.Information, LogLevel.Warning, true)]
        [InlineData(LogLevel.Warning, LogLevel.Debug, false)]
        [InlineData(LogLevel.Warning, LogLevel.Information, false)]
        [InlineData(LogLevel.Warning, LogLevel.Warning, true)]
        [InlineData(LogLevel.Warning, LogLevel.Error, true)]
        public void SetMinimumLevel_WriteEnabled(LogLevel minimumLevel, LogLevel logLevel, bool enabled)
        {
            // Arrange
            var options = new TestLoggerOptions();

            // Act
            options.FilterByMinimumLevel(minimumLevel);

            // Assert
            Assert.NotNull(options.WriteEnabled);
            Assert.Equal(enabled, options.WriteEnabled!(new LogEntry(logLevel, 0, null, null, string.Empty, string.Empty, Array.Empty<Scope>())));
        }

        [Theory]
        [InlineData("a.b.c", "a.b.c", true)]
        [InlineData("a.b.c", "a.b", false)]
        [InlineData("a.b.c", "a.b.c.d", false)]
        [InlineData("a.b.c", "a", false)]
        public void FilterByLoggerName_WriteEnabled(string loggerNameFilter, string loggerName, bool enabled)
        {
            // Arrange
            var options = new TestLoggerOptions();

            // Act
            options.FilterByLoggerName(loggerNameFilter);

            // Assert
            Assert.NotNull(options.WriteEnabled);
            Assert.Equal(enabled, options.WriteEnabled!(new LogEntry(0, 0, null, null, loggerName, string.Empty, Array.Empty<Scope>())));
        }

        [Theory]
        [InlineData("a.b.c", "a.b.c", true)]
        [InlineData("a.b.c", "a.b", false)]
        [InlineData("a.b.c", "a.b.c.d", false)]
        [InlineData("a.b.c", "a", false)]
        public void FilterByLoggerName_BeginEnabled(string loggerNameFilter, string loggerName, bool enabled)
        {
            // Arrange
            var options = new TestLoggerOptions();

            // Act
            options.FilterByLoggerName(loggerNameFilter);

            // Assert
            Assert.NotNull(options.BeginEnabled);
            Assert.Equal(enabled, options.BeginEnabled!(new BeginScope(loggerName, null)));
        }

        [Theory]
        [InlineData("A.B.C", true)]
        [InlineData("A.B", false)]
        [InlineData("A.B.C.D", false)]
        [InlineData("A", false)]
        public void FilterByTypeName_WriteEnabled(string loggerName, bool enabled)
        {
            // Arrange
            var options = new TestLoggerOptions();

            // Act
            options.FilterByTypeName<C>();

            // Assert
            Assert.NotNull(options.WriteEnabled);
            Assert.Equal(enabled, options.WriteEnabled!(new LogEntry(0, 0, null, null, loggerName, string.Empty, Array.Empty<Scope>())));
        }

        [Theory]
        [InlineData("A.B.C", true)]
        [InlineData("A.B", false)]
        [InlineData("A.B.C.D", false)]
        [InlineData("A", false)]
        public void FilterByTypeName_BeginEnabled(string loggerName, bool enabled)
        {
            // Arrange
            var options = new TestLoggerOptions();

            // Act
            options.FilterByTypeName<C>();

            // Assert
            Assert.NotNull(options.BeginEnabled);
            Assert.Equal(enabled, options.BeginEnabled!(new BeginScope(loggerName, null)));
        }

        [Theory]
        [InlineData("a.b", "a.b.c", true)]
        [InlineData("a.b", "a.b", false)]
        [InlineData("a.b", "a.b.c.d", true)]
        [InlineData("a.b", "a", false)]
        public void FilterByNamespace_WriteEnabled(string namespaceFilter, string loggerName, bool enabled)
        {
            // Arrange
            var options = new TestLoggerOptions();

            // Act
            options.FilterByNamespace(namespaceFilter);

            // Assert
            Assert.NotNull(options.WriteEnabled);
            Assert.Equal(enabled, options.WriteEnabled!(new LogEntry(0, 0, null, null, loggerName, string.Empty, Array.Empty<Scope>())));
        }

        [Theory]
        [InlineData("a.b", "a.b.c", true)]
        [InlineData("a.b", "a.b", false)]
        [InlineData("a.b", "a.b.c.d", true)]
        [InlineData("a.b", "a", false)]
        public void FilterByNamespace_BeginEnabled(string namespaceFilter, string loggerName, bool enabled)
        {
            // Arrange
            var options = new TestLoggerOptions();

            // Act
            options.FilterByNamespace(namespaceFilter);

            // Assert
            Assert.NotNull(options.BeginEnabled);
            Assert.Equal(enabled, options.BeginEnabled!(new BeginScope(loggerName, null)));
        }

        [Fact]
        public void EndToEndFiltering_ShouldApplyChainedFiltersToEntriesAndScopes()
        {
            // Arrange
            using var loggerFactory = TestLoggerFactory.Create(options => options
                .FilterByNamespace("A.B")
                .FilterByMinimumLevel(LogLevel.Warning));
            var matchingLogger = loggerFactory.CreateLogger<C>();
            var otherLogger = loggerFactory.CreateLogger("Other.Logger");

            // Act
            using (otherLogger.BeginScope("Ignored scope"))
            {
                otherLogger.LogError("Ignored error");
            }

            using (matchingLogger.BeginScope("Captured scope"))
            {
                matchingLogger.LogInformation("Ignored info");
                matchingLogger.LogWarning("Captured warning {value}", 42);
            }

            // Assert
            var scope = Assert.Single(loggerFactory.Sink.Scopes);
            Assert.Equal("Captured scope", scope.Message);
            Assert.Equal(typeof(C).FullName, scope.LoggerName);

            var entry = Assert.Single(loggerFactory.Sink.LogEntries);
            Assert.Equal("Captured warning 42", entry.Message);
            Assert.Equal("Captured warning {value}", entry.OriginalFormat);
            Assert.Equal(LogLevel.Warning, entry.LogLevel);
            var entryScope = Assert.Single(entry.Scopes);
            Assert.Equal("Captured scope", entryScope.Message);
        }

        [Fact]
        public void Clear_ShouldRemoveCapturedEntriesAndScopes_AndAllowSubsequentCapture()
        {
            // Arrange
            using var loggerFactory = TestLoggerFactory.Create();
            var logger = loggerFactory.CreateLogger("Test");

            using (logger.BeginScope("Scope 1"))
            {
                logger.LogInformation("Message 1");
            }

            // Act
            loggerFactory.Sink.Clear();

            // Assert
            Assert.Empty(loggerFactory.Sink.LogEntries);
            Assert.Empty(loggerFactory.Sink.Scopes);

            using (logger.BeginScope("Scope 2"))
            {
                logger.LogInformation("Message 2");
            }

            var entry = Assert.Single(loggerFactory.Sink.LogEntries);
            Assert.Equal("Message 2", entry.Message);
            var scope = Assert.Single(loggerFactory.Sink.Scopes);
            Assert.Equal("Scope 2", scope.Message);
        }

        [Fact]
        public void TestLoggerProvider_ParameterlessConstructor_ShouldCaptureAndClear()
        {
            // Arrange
            using var provider = new TestLoggerProvider();
            var logger = provider.CreateLogger("Direct");

            // Act
            using (logger.BeginScope("Direct scope"))
            {
                logger.LogInformation("Direct message");
            }

            // Assert
            var entry = Assert.Single(provider.Sink.LogEntries);
            Assert.Equal("Direct", entry.LoggerName);
            Assert.Equal("Direct message", entry.Message);
            var entryScope = Assert.Single(entry.Scopes);
            Assert.Equal("Direct scope", entryScope.Message);

            var scope = Assert.Single(provider.Sink.Scopes);
            Assert.Equal("Direct scope", scope.Message);

            provider.Sink.Clear();
            Assert.Empty(provider.Sink.LogEntries);
            Assert.Empty(provider.Sink.Scopes);
        }

        [Fact]
        public void GetTestLoggerSink_ShouldReturnSinkForTestLoggerFactory_AndThrowForOtherFactories()
        {
            // Arrange
            using var testFactory = TestLoggerFactory.Create();
            using var standardFactory = LoggerFactory.Create(_ => { });

            // Act & Assert
            Assert.Same(testFactory.Sink, testFactory.GetTestLoggerSink());
            Assert.Throws<ArgumentException>(() => standardFactory.GetTestLoggerSink());
        }

        [Fact]
        public void AddTest_CalledTwice_ShouldReplacePreviousRegistration()
        {
            // Arrange
            var services = new ServiceCollection();
            services.AddLogging(builder => builder.AddTest(options => options.FilterByLoggerName("First")));
            services.AddLogging(builder => builder.AddTest(options => options.FilterByLoggerName("Second")));

            using var provider = services.BuildServiceProvider();
            var factory = provider.GetRequiredService<ILoggerFactory>();
            var sink = provider.GetRequiredService<ITestLoggerSink>();

            // Act
            factory.CreateLogger("First").LogInformation("From first");
            factory.CreateLogger("Second").LogInformation("From second");

            // Assert
            var entry = Assert.Single(sink.LogEntries);
            Assert.Equal("From second", entry.Message);
        }
    }
}

namespace A.B
{
    class C { };
}
