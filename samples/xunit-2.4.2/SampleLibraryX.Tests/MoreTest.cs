using MELT;
using Microsoft.Extensions.Logging;
using Xunit;

namespace SampleLibraryX.Tests
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
            Assert.Collection(loggerFactory.Sink.LogEntries,
                l => Assert.Equal("More is less.", l.Message),
                l => Assert.Equal("The answer is 42", l.Message));
        }

        [Fact]
        public void DoMoreLogsMessage_NotCheckingNested()
        {
            // Arrange
            var loggerFactory = TestLoggerFactory.Create(options => options.FilterByTypeName<More>());
            var sampleLogger = loggerFactory.CreateLogger<Sample>();
            var moreLogger = loggerFactory.CreateLogger<More>();
            var more = new More(new Sample(sampleLogger), moreLogger);

            // Act
            more.DoMore();

            // Assert
            var log = Assert.Single(loggerFactory.Sink.LogEntries);
            Assert.Equal("More is less.", log.Message);
        }

        [Fact]
        public void DoMoreLogsFormat_NotCheckingNested()
        {
            // Arrange
            var loggerFactory = TestLoggerFactory.Create(options => options.FilterByTypeName<More>());
            var sampleLogger = loggerFactory.CreateLogger<Sample>();
            var moreLogger = loggerFactory.CreateLogger<More>();
            var more = new More(new Sample(sampleLogger), moreLogger);

            // Act
            more.DoMore();

            // Assert
            var log = Assert.Single(loggerFactory.Sink.LogEntries);
            Assert.Equal("More is less.", log.OriginalFormat);
        }

        [Fact]
        public void DoEvenMoreLogsCorrectParameters()
        {
            // Arrange
            var loggerFactory = TestLoggerFactory.Create();
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

        [Fact]
        public void UseScopeLogsScope()
        {
            // Arrange
            var loggerFactory = TestLoggerFactory.Create();
            var sampleLogger = loggerFactory.CreateLogger<Sample>();
            var moreLogger = loggerFactory.CreateLogger<More>();
            var more = new More(new Sample(sampleLogger), moreLogger);

            // Act
            more.UseScope();

            // Assert
            Assert.Collection(loggerFactory.Sink.Scopes,
                scope => Assert.Equal("This scope's answer is 42", scope.Message));
        }

        [Fact]
        public void UseScopeLogsCorrectParameters()
        {
            // Arrange
            var loggerFactory = TestLoggerFactory.Create();
            var sampleLogger = loggerFactory.CreateLogger<Sample>();
            var moreLogger = loggerFactory.CreateLogger<More>();
            var more = new More(new Sample(sampleLogger), moreLogger);

            // Act
            more.UseScope();

            // Assert
            var scope = Assert.Single(loggerFactory.Sink.Scopes);
            // Assert specific parameters in the log entry
            LoggingAssert.Contains("number", 42, scope.Properties);
        }

        [Fact]
        public void UseScopeLogsCorrectOriginalFormat()
        {
            // Arrange
            var loggerFactory = TestLoggerFactory.Create();
            var sampleLogger = loggerFactory.CreateLogger<Sample>();
            var moreLogger = loggerFactory.CreateLogger<More>();
            var more = new More(new Sample(sampleLogger), moreLogger);

            // Act
            more.UseScope();

            // Assert
            var scope = Assert.Single(loggerFactory.Sink.Scopes);
            // Assert specific parameters in the log entry
            Assert.Equal("This scope's answer is {number}", scope.OriginalFormat);
        }

        [Fact]
        public void UseLocalScopeLogsMessageWithScope()
        {
            // Arrange
            var loggerFactory = TestLoggerFactory.Create();
            var sampleLogger = loggerFactory.CreateLogger<Sample>();
            var moreLogger = loggerFactory.CreateLogger<More>();
            var more = new More(new Sample(sampleLogger), moreLogger);

            // Act
            more.UseLocalScope();

            // Assert
            var log = Assert.Single(loggerFactory.Sink.LogEntries);
            var scope = Assert.Single(log.Scopes);
            Assert.Equal("This scope's answer is 42", scope.Message);
        }

        [Fact]
        public void TraceLogsMessageWithScope()
        {
            // Arrange
            var loggerFactory = TestLoggerFactory.Create();
            var sampleLogger = loggerFactory.CreateLogger<Sample>();
            var moreLogger = loggerFactory.CreateLogger<More>();
            var more = new More(new Sample(sampleLogger), moreLogger);

            // Act
            more.Trace();

            // Assert
            var log = Assert.Single(loggerFactory.Sink.LogEntries);
            Assert.Equal("This log entry is at trace level", log.Message);
            LoggingAssert.Contains("level", "trace", log.Properties);
            var scope = Assert.Single(log.Scopes);
            LoggingAssert.Contains("foo", "bar", scope.Properties);
        }
    }
}
