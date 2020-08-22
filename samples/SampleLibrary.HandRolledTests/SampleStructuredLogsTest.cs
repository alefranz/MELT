using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using SampleLibrary;
using Xunit;

#nullable disable

namespace SampleLibraryHandRolledTests
{
    public class SampleStructuredLogsTest
    {
        [Fact]
        public void DoSomethingLogsMessage()
        {
            // Arrange
            var logger = new FakeLogger();
            var sample = new Sample(logger);

            // Act
            sample.DoSomething();

            // Assert
            var log = Assert.Single(logger.Entries);
            // Assert the message rendered by a default formatter
            Assert.Equal("The answer is 42", log.Message);

            var element = Assert.Single(log.Properties, x => x.Key == "number");
            Assert.Equal(42, element.Value);
            // or
            Assert.Contains(new KeyValuePair<string, object>("number", 42), log.Properties);

            var template = Assert.Single(log.Properties, x => x.Key == "{OriginalFormat}");
            Assert.Equal("The answer is {number}", template.Value);
        }

        [Fact]
        public void DoExceptionalLogsException()
        {
            // Arrange
            var logger = new FakeLogger();
            var sample = new Sample(logger);

            // Act
            sample.DoExceptional();

            // Assert
            var log = Assert.Single(logger.Entries);

            // Assert the message rendered by default
            Assert.Equal("There was a problem", log.Message);

            var exception = Assert.IsType<ArgumentNullException>(log.Exception);
            Assert.Equal("foo", exception.ParamName);
        }

        private class FakeLogger : ILogger<Sample>
        {
            public List<(LogLevel Level, string Message, IReadOnlyList<KeyValuePair<string, object>> Properties, Exception Exception)> Entries { get; } =
                new List<(LogLevel, string, IReadOnlyList<KeyValuePair<string, object>>, Exception)>();

            public IDisposable BeginScope<TState>(TState state) => throw new NotImplementedException();

            public bool IsEnabled(LogLevel logLevel) => throw new NotImplementedException();

            public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
            {
                // These are relying on an internal implementation detail, they will break!
                var message = state.ToString();
                var properties = state as IReadOnlyList<KeyValuePair<string, object>>;

                Entries.Add((logLevel, message, properties, exception));
            }
        }
    }
}
