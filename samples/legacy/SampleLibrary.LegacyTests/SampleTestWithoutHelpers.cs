using System.Collections.Generic;
using System.Linq;
using MELT;
using MELT.Xunit;
using Microsoft.Extensions.Logging;
using Xunit;

namespace SampleLibrary.LegacyTests
{
    public class SampleTestWithoutHelpers
    {
        [Fact]
        public void DoSomethingLogsMessage()
        {
            // Arrange
            var sink = new TestSink();
            var loggerFactory = new TestLoggerFactory(sink);
            var logger = loggerFactory.CreateLogger<Sample>();
            var sample = new Sample(logger);

            // Act
            sample.DoSomething();

            // Assert
            Assert.Single(sink.Writes);
            var log = sink.Writes.Single();
            // Assert the message rendered by a default formatter
            Assert.Equal("The answer is 42", log.Message);
        }

        [Fact]
        public void DoSomethingLogsUsingCorrectFormat()
        {
            // Arrange
            var sink = new TestSink();
            var loggerFactory = new TestLoggerFactory(sink);
            var logger = loggerFactory.CreateLogger<Sample>();
            var sample = new Sample(logger);

            // Act
            sample.DoSomething();

            // Assert
            Assert.Single(sink.Writes);
            var log = sink.Writes.Single();
            var state = Assert.IsAssignableFrom<IEnumerable<KeyValuePair<string, object>>>(log.State);
            // Assert the the log format template
            LogValuesAssert.Contains("{OriginalFormat}", "The answer is {number}", state);
        }

        [Fact]
        public void DoSomethingLogsCorrectParameter()
        {
            // Arrange
            var sink = new TestSink();
            var loggerFactory = new TestLoggerFactory(sink);
            var logger = loggerFactory.CreateLogger<Sample>();
            var sample = new Sample(logger);

            // Act
            sample.DoSomething();

            // Assert
            Assert.Single(sink.Writes);
            var log = sink.Writes.Single();
            var state = Assert.IsAssignableFrom<IEnumerable<KeyValuePair<string, object>>>(log.State);
            // Assert specific parameters in the log entry
            LogValuesAssert.Contains("number", 42, state);
        }
    }
}
