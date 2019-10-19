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
            var options = new TestSinkOptions();

            // Act
            options.SetMinimumLevel(minimumLevel);

            // Assert
            Assert.Equal(enabled, options.WriteEnabled(new WriteContext { LogLevel = logLevel }));
        }

        [Theory]
        [InlineData("a.b.c", "a.b.c", true)]
        [InlineData("a.b.c", "a.b", false)]
        [InlineData("a.b.c", "a.b.c.d", false)]
        [InlineData("a.b.c", "a", false)]
        public void FilterByLoggerName_WriteEnabled(string loggerNameFilter, string loggerName, bool enabled)
        {
            // Arrange
            var options = new TestSinkOptions();

            // Act
            options.FilterByLoggerName(loggerNameFilter);

            // Assert
            Assert.Equal(enabled, options.WriteEnabled(new WriteContext { LoggerName = loggerName }));
        }

        [Theory]
        [InlineData("a.b", "a.b.c", true)]
        [InlineData("a.b", "a.b", false)]
        [InlineData("a.b", "a.b.c.d", true)]
        [InlineData("a.b", "a", false)]
        public void FilterByNamespace_WriteEnabled(string namespaceFilter, string loggerName, bool enabled)
        {
            // Arrange
            var options = new TestSinkOptions();

            // Act
            options.FilterByNamespace(namespaceFilter);

            // Assert
            Assert.Equal(enabled, options.WriteEnabled(new WriteContext { LoggerName = loggerName }));
        }

        [Theory]
        [InlineData("a.b.c", "a.b.c", true)]
        [InlineData("a.b.c", "a.b", false)]
        [InlineData("a.b.c", "a.b.c.d", false)]
        [InlineData("a.b.c", "a", false)]
        public void FilterByLoggerName_BeginEnabled(string loggerNameFilter, string loggerName, bool enabled)
        {
            // Arrange
            var options = new TestSinkOptions();

            // Act
            options.FilterByLoggerName(loggerNameFilter);

            // Assert
            Assert.Equal(enabled, options.BeginEnabled(new BeginScopeContext { LoggerName = loggerName }));
        }

        [Theory]
        [InlineData("a.b", "a.b.c", true)]
        [InlineData("a.b", "a.b", false)]
        [InlineData("a.b", "a.b.c.d", true)]
        [InlineData("a.b", "a", false)]
        public void FilterByNamespace_BeginEnabled(string namespaceFilter, string loggerName, bool enabled)
        {
            // Arrange
            var options = new TestSinkOptions();

            // Act
            options.FilterByNamespace(namespaceFilter);

            // Assert
            Assert.Equal(enabled, options.BeginEnabled(new BeginScopeContext { LoggerName = loggerName }));
        }
    }
}
