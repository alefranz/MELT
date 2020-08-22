using System;
using MELT;
using Microsoft.Extensions.Logging;
using Xunit;

namespace SampleLibrary.Tests
{
    public class SampleTest
    {
        [Fact]
        public void DoSomethingLogsMessage()
        {
            // Arrange
            var loggerFactory = TestLoggerFactory.Create();

            var logger = loggerFactory.CreateLogger<Sample>();
            var sample = new Sample(logger);

            // Act
            sample.DoSomething();

            // Assert
            var log = Assert.Single(loggerFactory.Sink.LogEntries);
            // Assert the message rendered by a default formatter
            Assert.Equal("The answer is 42", log.Message);
        }

        [Fact]
        public void DoSomethingLogsCorrectParameter()
        {
            // Arrange
            var loggerFactory = TestLoggerFactory.Create();
            var logger = loggerFactory.CreateLogger<Sample>();
            var sample = new Sample(logger);

            // Act
            sample.DoSomething();

            // Assert
            var log = Assert.Single(loggerFactory.Sink.LogEntries);
            // Assert specific parameters in the log entry
            LoggingAssert.Contains("number", 42, log.Properties);
        }

        [Fact]
        public void DoSomethingLogsUsingCorrectFormat()
        {
            // Arrange
            var loggerFactory = TestLoggerFactory.Create();
            var logger = loggerFactory.CreateLogger<Sample>();
            var sample = new Sample(logger);

            // Act
            sample.DoSomething();

            // Assert
            var log = Assert.Single(loggerFactory.Sink.LogEntries);
            // Assert the the log format template
            Assert.Equal("The answer is {number}", log.OriginalFormat);
        }

        [Fact]
        public void DoExceptionalLogsException()
        {
            // Arrange
            var loggerFactory = TestLoggerFactory.Create();
            var logger = loggerFactory.CreateLogger<Sample>();
            var sample = new Sample(logger);

            // Act
            sample.DoExceptional();

            // Assert
            var log = Assert.Single(loggerFactory.Sink.LogEntries);
            // Assert the message rendered by a default formatter
            Assert.Equal("There was a problem", log.Message);
            // Assert specific parameters in the log entry
            LoggingAssert.Contains("error", "problem", log.Properties);
            // Assert the exception
            var exception = Assert.IsType<ArgumentNullException>(log.Exception);
            Assert.Equal("foo", exception.ParamName);
        }
    }
}
