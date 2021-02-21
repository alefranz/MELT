using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
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
            var scope = Assert.Single(entry.Scopes);

            Assert.Equal("Scope 1", scope.Message);
            Assert.Equal("Message 1", entry.Message);
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

            Assert.Empty(entry.Scopes);
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

            Assert.Equal(new[] { "Scope 1", "Scope 2" }, entry.Scopes.Select(x => x.Message));
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
            var expectations = new List<(string expectedMessage, string[] expectedScopes)>()
            {
                ("Message 1", new []{ "Outer Scope" } ),
                ("Message 2", new []{ "Outer Scope", "Inner Scope" } ),
                ("Message 3", new []{ "Outer Scope" } )
            };

            Assert.Equal(expectations.Count, loggerFactory.Sink.LogEntries.Count());

            foreach (var (logEntry, (expectedMessage, expectedScope)) in loggerFactory.Sink.LogEntries.Zip(expectations))
            {
                Assert.Equal(logEntry.Message, expectedMessage);
                Assert.Equal(expectedScope, logEntry.Scopes.Select(x => x.Message));
                Assert.Equal(expectedMessage, logEntry.Message);
            }
        }

        [Fact]
        public void ClosingAScope_ShouldCloseAllInnerScopes_AndRestoreParentScope()
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
            // Disposing B also removes C
            scopeB.Dispose();
            logger.LogInformation("Message 4");
            // Disposing C restore B (as well as its parent A)
            scopeC.Dispose();
            logger.LogInformation("Message 5");
            // Disposing A also removes B
            scopeA.Dispose();
            logger.LogInformation("Message 6");

            //Assert
            var expectations = new List<(string expectedMessage, string[] expectedScopes)>()
            {
                ("Message 1", new []{ "Scope A" } ),
                ("Message 2", new []{ "Scope A", "Scope B" } ),
                ("Message 3", new []{ "Scope A", "Scope B","Scope C"} ),
                ("Message 4", new []{ "Scope A" } ),
                ("Message 5", new []{ "Scope A", "Scope B" } ),
                ("Message 6", new string[]{} ),
            };

            Assert.Equal(expectations.Count, loggerFactory.Sink.LogEntries.Count());

            foreach (var (logEntry, (expectedMessage, expectedScope)) in loggerFactory.Sink.LogEntries.Zip(expectations))
            {
                Assert.Equal(logEntry.Message, expectedMessage);
                Assert.Equal(expectedScope, logEntry.Scopes.Select(x => x.Message));
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
                loggerA.LogInformation("Message A1");
                loggerB.LogInformation("Message B1");
                using (loggerB.BeginScope("B Scope"))
                {
                    loggerA.LogInformation("Message A2");  // only in A as B is in different logger
                    loggerB.LogInformation("Message B2");
                }
                loggerA.LogInformation("Message A3");
                loggerB.LogInformation("Message B3");
            }

            loggerA.LogInformation("Message A4");
            loggerB.LogInformation("Message B4");

            //Assert
            var expectations = new List<(string expectedMessage, string[] expectedScopes)>()
            {
                ("Message A1", new []{ "A Scope" } ),
                ("Message B1", Array.Empty<string>() ),
                ("Message A2", new []{ "A Scope" } ),
                ("Message B2", new []{ "B Scope" } ),
                ("Message A3", new []{ "A Scope" } ),
                ("Message B3", Array.Empty<string>() ),
                ("Message A4", Array.Empty<string>() ),
                ("Message B4", Array.Empty<string>() ),
            };

            Assert.Equal(expectations.Count, loggerFactory.Sink.LogEntries.Count());

            foreach (var (logEntry, (expectedMessage, expectedScope)) in loggerFactory.Sink.LogEntries.Zip(expectations))
            {
                Assert.Equal(logEntry.Message, expectedMessage);
                Assert.Equal(expectedScope, logEntry.Scopes.Select(x => x.Message));
                Assert.Equal(expectedMessage, logEntry.Message);
            }
        }

        [Fact]
        public async Task MessagesLoggedInScopesInDifferentAsyncScopes_ShouldHaveCorrectScopes()
        {
            static async Task LogAsync(SemaphoreSlim scopeCreated, SemaphoreSlim ready, ILogger logger,
                string name, CancellationToken cancellationToken)
            {
                using (logger.BeginScope(name))
                {
                    scopeCreated.Release();
                    await ready.WaitAsync(cancellationToken);
                    logger.LogInformation(name);
                }
            }

            //Arrange
            var loggerFactory = TestLoggerFactory.Create();
            var logger = loggerFactory.CreateLogger("Test");
            var semaphoreA = new SemaphoreSlim(0, 1);
            var semaphoreB = new SemaphoreSlim(0, 1);
            using var cts = Debugger.IsAttached ? new CancellationTokenSource() : new CancellationTokenSource(TimeSpan.FromSeconds(2));

            //Act
            var taskA = LogAsync(semaphoreB, semaphoreA, logger, "A", cts.Token);
            var taskB = LogAsync(semaphoreA, semaphoreB, logger, "B", cts.Token);

            await Task.WhenAll(taskA, taskB);

            //Assert
            Assert.Equal(2, loggerFactory.Sink.LogEntries.Count());
            Assert.All(loggerFactory.Sink.LogEntries,
                x => Assert.Equal(x.Message, x.Scopes.Select(x => x.Message).Single()));
        }

        [Fact]
        public void MessageLoggedInsideScopeWithProperties_ShouldHaveScopeWithProperties()
        {
            //Arrange
            var loggerFactory = TestLoggerFactory.Create();
            var logger = loggerFactory.CreateLogger("Test");

            //Act
            using (logger.BeginScope(new Dictionary<string, object>
            {
                ["foo"] = "bar",
                ["answer"] = 42
            }))
            {
                logger.LogInformation("Message 1");
            }

            //Assert
            var entry = Assert.Single(loggerFactory.Sink.LogEntries);
            var scope = Assert.Single(entry.Scopes);

            Assert.Equal(new Dictionary<string, object>
            {
                ["foo"] = "bar",
                ["answer"] = 42
            }, scope.Properties);
            Assert.Equal("Message 1", entry.Message);
        }
    }
}
