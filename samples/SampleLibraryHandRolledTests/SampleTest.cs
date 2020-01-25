using Microsoft.Extensions.Logging;
using SampleLibrary;
using Xunit;
using System.Collections.Generic;
using System;

#nullable disable

namespace SampleLibraryHandRolledTests
{
    public class SampleTest
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
            // Assert the message rendered by a default formatter
            Assert.Equal("There was a problem", log.Message);
        }

        private class FakeLogger : ILogger<Sample>
        {
            public List<(LogLevel Level, string Message)> Entries { get; } = new List<(LogLevel, string)>();

            public IDisposable BeginScope<TState>(TState state)
            {
                throw new NotImplementedException();
            }

            public bool IsEnabled(LogLevel logLevel)
            {
                throw new NotImplementedException();
            }

            public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
            {
                Entries.Add((logLevel, state.ToString())));
            }
        }
    }
}
