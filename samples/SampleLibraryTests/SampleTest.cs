using Microsoft.Extensions.Logging;
using MELT;
using SampleLibrary;
using Xunit;
using MELT.Xunit;
using System.Collections.Generic;

namespace SampleLibraryTests
{
    public class SampleTest
    {
        [Fact]
        public void DoSomethingLogsMessage()
        {
            // Arrange
            var loggerFactory = MELTBuilder.CreateLoggerFactory();
            var logger = loggerFactory.CreateLogger<Sample>();
            var sample = new Sample(logger);

            // Act
            sample.DoSomething();

            // Assert
            var log = Assert.Single(loggerFactory.LogEntries);
            // Assert the message rendered by a default formatter
            Assert.Equal("The answer is 42", log.Message);
        }

        [Fact]
        public void DoSomethingLogsCorrectParameter()
        {
            // Arrange
            var loggerFactory = MELTBuilder.CreateLoggerFactory();
            var logger = loggerFactory.CreateLogger<Sample>();
            var sample = new Sample(logger);

            // Act
            sample.DoSomething();

            // Assert
            var log = Assert.Single(loggerFactory.LogEntries);
            // Assert specific parameters in the log entry
            LogValuesAssert.Contains("number", 42, log.Properties);
            Assert.Contains(new KeyValuePair<string, object>("number", 42), log.Properties);
        }
    }
}
