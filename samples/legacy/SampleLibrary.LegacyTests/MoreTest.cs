using MELT;
using Microsoft.Extensions.Logging;
using SampleLibrary;
using Xunit;

namespace SampleLibrary.LegacyTests
{
    public class MoreTest
    {
        [Fact]
        public void DoMoreLogsMessage()
        {
            // Arrange
            var loggerFactory = MELTBuilder.CreateLoggerFactory();
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
    }
}
