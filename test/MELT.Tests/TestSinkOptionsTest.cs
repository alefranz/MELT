using A.B;
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
            options.FilterByMinimumLevel(minimumLevel);

            // Assert
            Assert.Equal(enabled, options.WriteEnabled(new WriteContext(logLevel, 0, null, null, null, string.Empty, string.Empty)));
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
            Assert.Equal(enabled, options.WriteEnabled(new WriteContext(0, 0, null, null, null, loggerName, string.Empty)));
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
            Assert.Equal(enabled, options.BeginEnabled(new BeginScopeContext(loggerName, null)));
        }

        [Theory]
        [InlineData("A.B.C", true)]
        [InlineData("A.B", false)]
        [InlineData("A.B.C.D", false)]
        [InlineData("A", false)]
        public void FilterByTypeName_WriteEnabled(string loggerName, bool enabled)
        {
            // Arrange
            var options = new TestSinkOptions();

            // Act
            options.FilterByTypeName<C>();

            // Assert
            Assert.Equal(enabled, options.WriteEnabled(new WriteContext(0, 0, null, null, null, loggerName, string.Empty)));
        }

        [Theory]
        [InlineData("A.B.C", true)]
        [InlineData("A.B", false)]
        [InlineData("A.B.C.D", false)]
        [InlineData("A", false)]
        public void FilterByTypeName_BeginEnabled(string loggerName, bool enabled)
        {
            // Arrange
            var options = new TestSinkOptions();

            // Act
            options.FilterByTypeName<C>();

            // Assert
            Assert.Equal(enabled, options.BeginEnabled(new BeginScopeContext(loggerName, null)));
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
            Assert.Equal(enabled, options.WriteEnabled(new WriteContext(0, 0, null, null, null, loggerName, string.Empty)));
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
            Assert.Equal(enabled, options.BeginEnabled(new BeginScopeContext(loggerName, null)));
        }
    }
}

namespace A.B
{
    class C { };
}
