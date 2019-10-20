using Microsoft.Extensions.Logging;
using MELT;
using SampleLibrary;
using Xunit;
using System.Linq;
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
            var loggerFactory = new TestLoggerFactory();
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
            var loggerFactory = TestLoggerFactoryBuilder.CreateLoggerFactory();
            var logger = loggerFactory.CreateLogger<Sample>();
            var sample = new Sample(logger);

            // Act
            sample.DoSomething();

            // Assert
            var log = Assert.Single(loggerFactory.LogEntries);
            // Assert specific parameters in the log entry
            LogValuesAssert.Contains("number", 42, log.Properties);
        }
    }
}
