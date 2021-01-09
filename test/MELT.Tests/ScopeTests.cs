using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Extensions.Logging;
using Xunit;

namespace MELT.Tests
{
    public class ScopeTests
    {
        [Fact]
        public void MessageLoggedInsideScope_ShouldHaveScope()
        {
            //Arrange
            var loggerFactory = TestLoggerFactory.Create();
            var logger = loggerFactory.CreateLogger("Test");

            //Act
            using (logger.BeginScope("Scope 1"))
            {
                logger.LogInformation("Message 1");
            }

            //Assert
            var entry = Assert.Single(loggerFactory.Sink.LogEntries);

            var scope = Assert.Single(entry.FullScope);

            Assert.Equal("Scope 1",scope.Message);
            Assert.Equal("Message 1",entry.Message);
        }

        [Fact]
        public void MessageLoggedOutsideScope_ShouldHaveEmptyScopeStack()
        {
            //Arrange
            var loggerFactory = TestLoggerFactory.Create();
            var logger = loggerFactory.CreateLogger("Test");

            //Act
            using (logger.BeginScope("Scope 1"))
            {
            }
            logger.LogInformation("Message 1");

            //Assert
            var entry = Assert.Single(loggerFactory.Sink.LogEntries);

            Assert.Empty(entry.FullScope);
            Assert.Equal("Message 1", entry.Message);
        }

        [Fact]
        public void MessagesLoggedInNestedScopes_ShouldHaveAllScopes()
        {
            //Arrange
            var loggerFactory = TestLoggerFactory.Create();
            var logger = loggerFactory.CreateLogger("Test");

            //Act
            using (logger.BeginScope("Scope 1"))
            {
                using (logger.BeginScope("Scope 2"))
                {
                    logger.LogInformation("Message 1");
                }
            }

            //Assert
            var entry = Assert.Single(loggerFactory.Sink.LogEntries);

            Assert.Equal(new[]{"Scope 2", "Scope 1"}, entry.FullScope.Select(x=>x.Message));


            Assert.Equal("Message 1", entry.Message);
        }



        [Fact]
        public void MessagesLoggedInScopes_ShouldHaveCorrectScopes()
        {
            //Arrange
            var loggerFactory = TestLoggerFactory.Create();
            var logger = loggerFactory.CreateLogger("Test");

            //Act

            using (logger.BeginScope("Outer Scope"))
            {
                logger.LogInformation("Message 1");

                using (logger.BeginScope("Inner Scope"))
                {
                    logger.LogInformation("Message 2");
                }

                logger.LogInformation("Message 3");
            }

            //Assert

            var expectations = new List<(string expectedMessage,  string[]  expectedScopes)>()
            {
                ("Message 1", new []{"Outer Scope"} ),
                ("Message 2", new []{"Inner Scope","Outer Scope"} ),
                ("Message 3", new []{"Outer Scope"} )
            };

            Assert.Equal(expectations.Count, loggerFactory.Sink.LogEntries.Count());


            foreach (var (logEntry, (expectedMessage, expectedScope)) in loggerFactory.Sink.LogEntries.Zip(expectations))
            {
                Assert.Equal(logEntry.Message, expectedMessage);


                Assert.Equal(expectedScope, logEntry.FullScope.Select(x=>x.Message));

                Assert.Equal(expectedMessage, logEntry.Message);
            }
        }

        [Fact]
        public void ClosingAScope_ShouldCloseAllInnerScopes()
        {
            //Arrange
            var loggerFactory = TestLoggerFactory.Create();
            var logger = loggerFactory.CreateLogger("Test");

            //Act

            var scopeA = logger.BeginScope("Scope A");
            logger.LogInformation("Message 1");
            var scopeB = logger.BeginScope("Scope B");
            logger.LogInformation("Message 2");
            var scopeC = logger.BeginScope("Scope C");
            logger.LogInformation("Message 3");
            scopeB.Dispose();
            logger.LogInformation("Message 4");
            scopeC.Dispose();
            logger.LogInformation("Message 5");
            scopeA.Dispose();
            logger.LogInformation("Message 6");

            //Assert

            var expectations = new List<(string expectedMessage, string[] expectedScopes)>()
            {
                ("Message 1", new []{"Scope A"} ),
                ("Message 2", new []{"Scope B","Scope A"} ),
                ("Message 3", new []{"Scope C", "Scope B","Scope A"} ),
                ("Message 4", new []{"Scope A"} ),
                ("Message 5", new []{"Scope A"} ),
                ("Message 6", new string[]{} ),
            };

            Assert.Equal(expectations.Count, loggerFactory.Sink.LogEntries.Count());


            foreach (var (logEntry, (expectedMessage, expectedScope)) in loggerFactory.Sink.LogEntries.Zip(expectations))
            {
                Assert.Equal(logEntry.Message, expectedMessage);


                Assert.Equal(expectedScope, logEntry.FullScope.Select(x => x.Message));

                Assert.Equal(expectedMessage, logEntry.Message);
            }
        }

        [Fact]
        public void MessagesLoggedInScopesByDifferentLoggers_ShouldHaveCorrectScopes()
        {
            //Arrange
            var loggerFactory = TestLoggerFactory.Create();
            var loggerA = loggerFactory.CreateLogger("Test-A");
            var loggerB = loggerFactory.CreateLogger("Test-B");

            //Act

            using (loggerA.BeginScope("A Scope"))
            {
                loggerA.LogInformation("Message 1");

                using (loggerB.BeginScope("B Scope"))
                {
                    loggerA.LogInformation("Message 2");
                }

                loggerA.LogInformation("Message 3");
                loggerB.LogInformation("Message 4");
            }

            loggerA.LogInformation("Message 5");
            loggerB.LogInformation("Message 6");

            //Assert

            var expectations = new List<(string expectedMessage, string[] expectedScopes)>()
            {
                ("Message 1", new []{"A Scope"} ),
                ("Message 2", new []{"B Scope","A Scope"} ),
                ("Message 3", new []{"A Scope"} ),
                ("Message 4", new []{"A Scope"} ),
                ("Message 5", Array.Empty<string>() ),
                ("Message 6", Array.Empty<string>() ),
            };

            Assert.Equal(expectations.Count, loggerFactory.Sink.LogEntries.Count());


            foreach (var (logEntry, (expectedMessage, expectedScope)) in loggerFactory.Sink.LogEntries.Zip(expectations))
            {
                Assert.Equal(logEntry.Message, expectedMessage);


                Assert.Equal(expectedScope, logEntry.FullScope.Select(x => x.Message));

                Assert.Equal(expectedMessage, logEntry.Message);
            }
        }

    }
}
