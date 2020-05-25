using Microsoft.Extensions.Logging;
using MELT;
using Xunit;

namespace SampleLibrary.Tests
{
    public class MoreTest
    {
        [Fact]
        public void DoMoreLogsMessage()
        {
            // Arrange
            var loggerFactory = TestLoggerFactory.Create();
            var sampleLogger = loggerFactory.CreateLogger<Sample>();
            var moreLogger = loggerFactory.CreateLogger<More>();
            var more = new More(new Sample(sampleLogger), moreLogger);

            // Act
            more.DoMore();

            // Assert
            Assert.Collection(loggerFactory.LogEntries,
                l => Assert.Equal("More is less.", l.Message),
                l => Assert.Equal("The answer is 42", l.Message));
        }

        [Fact]
        public void DoMoreLogsMessage_NotCheckingNested()
        {
            // Arrange
            var loggerFactory = MELTBuilder.CreateLoggerFactory(options => options.FilterByTypeName<More>());
            var sampleLogger = loggerFactory.CreateLogger<Sample>();
            var moreLogger = loggerFactory.CreateLogger<More>();
            var more = new More(new Sample(sampleLogger), moreLogger);

            // Act
            more.DoMore();

            // Assert
            var log = Assert.Single(loggerFactory.LogEntries);
            Assert.Equal("More is less.", log.Message);
        }

        [Fact]
        public void DoMoreLogsFormat_NotCheckingNested()
        {
            // Arrange
            var loggerFactory = MELTBuilder.CreateLoggerFactory(options => options.FilterByTypeName<More>());
            var sampleLogger = loggerFactory.CreateLogger<Sample>();
            var moreLogger = loggerFactory.CreateLogger<More>();
            var more = new More(new Sample(sampleLogger), moreLogger);

            // Act
            more.DoMore();

            // Assert
            var log = Assert.Single(loggerFactory.LogEntries);
            Assert.Equal("More is less.", log.Format);
        }

        [Fact]
        public void DoEvenMoreLogsCorrectParameters()
        {
            // Arrange
            var loggerFactory = MELTBuilder.CreateLoggerFactory();
            var sampleLogger = loggerFactory.CreateLogger<Sample>();
            var moreLogger = loggerFactory.CreateLogger<More>();
            var more = new More(new Sample(sampleLogger), moreLogger);


            // Act
            more.DoEvenMore();

            // Assert
            var log = Assert.Single(loggerFactory.Sink.LogEntries);
            // Assert specific parameters in the log entry
            LoggingAssert.Contains("number", 42, log.Properties);
            LoggingAssert.Contains("foo", "bar", log.Properties);
        }
    }
}
