using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using Serilog.Extensions.Logging;
using Xunit;

namespace MELT.Serilog.Tests
{
    public class RegistrationAndSinkAccessorTests
    {
        [Fact]
        public void AddSerilogTest_CalledTwice_ShouldReplacePreviousRegistration()
            => AssertLastRegistrationWins(
                builder => builder.AddSerilogTest(options => options.FilterByLoggerName("First")),
                builder => builder.AddSerilogTest(options => options.FilterByLoggerName("Second")),
                clearThroughSerilogSink: true);

        [Fact]
        public void AddTest_ThenAddSerilogTest_ShouldUseLastRegistrationForBothSinks()
            => AssertLastRegistrationWins(
                builder => builder.AddTest(options => options.FilterByLoggerName("First")),
                builder => builder.AddSerilogTest(options => options.FilterByLoggerName("Second")),
                clearThroughSerilogSink: true);

        [Fact]
        public void AddSerilogTest_ThenAddTest_ShouldUseLastRegistrationForBothSinks()
            => AssertLastRegistrationWins(
                builder => builder.AddSerilogTest(options => options.FilterByLoggerName("First")),
                builder => builder.AddTest(options => options.FilterByLoggerName("Second")),
                clearThroughSerilogSink: false);

        [Fact]
        public async Task TryGetTestLoggerSink_And_TryGetSerilogTestLoggerSink_ShouldNotBeAmbiguous()
        {
            // Arrange & Act - neither registered
            using (var factory = new TestWebApplicationFactory())
            {
                Assert.False(factory.TryGetTestLoggerSink(out var standardSink));
                Assert.Null(standardSink);

                Assert.False(factory.TryGetSerilogTestLoggerSink(out var serilogSink));
                Assert.Null(serilogSink);
            }

            // Arrange & Act - standard test logging registered
            using (var baseFactory = new TestWebApplicationFactory())
            using (var factory = baseFactory.WithWebHostBuilder(builder =>
                builder.UseTestLogging(options => options.FilterByLoggerName("Test.Endpoint"))))
            {
                using var client = factory.CreateDefaultClient();
                using var response = await client.GetAsync("/");
                response.EnsureSuccessStatusCode();

                Assert.True(factory.TryGetTestLoggerSink(out var standardSink));
                Assert.NotNull(standardSink);
                var entry = Assert.Single(standardSink!.LogEntries);
                Assert.Equal("Handled /", entry.Message);

                Assert.False(factory.TryGetSerilogTestLoggerSink(out var serilogSink));
                Assert.Null(serilogSink);
            }

            // Arrange & Act - Serilog test logging registered
            using (var baseFactory = new TestWebApplicationFactory())
            using (var factory = baseFactory.WithWebHostBuilder(builder =>
                builder.UseSerilogTestLogging(options => options.FilterByLoggerName("Test.Endpoint"))))
            {
                using var client = factory.CreateDefaultClient();
                using var response = await client.GetAsync("/");
                response.EnsureSuccessStatusCode();

                Assert.True(factory.TryGetTestLoggerSink(out var standardSink));
                Assert.NotNull(standardSink);
                var standardEntry = Assert.Single(standardSink!.LogEntries);
                Assert.Equal("Handled /", standardEntry.Message);

                Assert.True(factory.TryGetSerilogTestLoggerSink(out var serilogSink));
                Assert.NotNull(serilogSink);
                var serilogEntry = Assert.Single(serilogSink!.LogEntries);
                Assert.Equal("Handled /", serilogEntry.Message);
            }
        }

        private static void AssertLastRegistrationWins(
            Action<ILoggingBuilder> configureFirst,
            Action<ILoggingBuilder> configureSecond,
            bool clearThroughSerilogSink)
        {
            // Arrange
            var services = new ServiceCollection();
            services.AddLogging(configureFirst);
            services.AddLogging(configureSecond);

            using var serviceProvider = services.BuildServiceProvider();
            var providers = new LoggerProviderCollection();
            foreach (var loggerProvider in serviceProvider.GetServices<ILoggerProvider>())
            {
                providers.AddProvider(loggerProvider);
            }

            using var serilogLogger = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .Enrich.FromLogContext()
                .WriteTo.Providers(providers)
                .CreateLogger();

            using ILoggerFactory loggerFactory = new SerilogLoggerFactory(serilogLogger, dispose: false, providers);
            var firstLogger = loggerFactory.CreateLogger("First");
            var secondLogger = loggerFactory.CreateLogger("Second");

            // Act
            using (firstLogger.BeginScope("First scope"))
            {
                firstLogger.LogInformation("Hello {target}", "First");
            }

            using (secondLogger.BeginScope("Second scope"))
            {
                secondLogger.LogInformation("Hello {target}", "Second");
            }

            // Assert
            Assert.Single(serviceProvider.GetServices<ILoggerProvider>().OfType<TestLoggerProvider>());

            var standardSink = serviceProvider.GetRequiredService<ITestLoggerSink>();
            var serilogSink = serviceProvider.GetRequiredService<ISerilogTestLoggerSink>();

            var standardEntry = Assert.Single(standardSink.LogEntries);
            Assert.Equal("Second", standardEntry.LoggerName);
            Assert.Equal("Hello \"Second\"", standardEntry.Message);

            var serilogEntry = Assert.Single(serilogSink.LogEntries);
            Assert.Equal("Second", serilogEntry.LoggerName);
            Assert.Equal("Hello \"Second\"", serilogEntry.Message);
            var serilogScope = Assert.Single(serilogEntry.Scopes);
            Assert.Equal(new ScalarValue("Second scope"), serilogScope);

            if (clearThroughSerilogSink)
            {
                serilogSink.Clear();
            }
            else
            {
                standardSink.Clear();
            }

            Assert.Empty(standardSink.LogEntries);
            Assert.Empty(serilogSink.LogEntries);
        }

        public sealed class TestAppMarker
        {
        }

        private sealed class TestWebApplicationFactory : WebApplicationFactory<TestAppMarker>
        {
            protected override IHostBuilder CreateHostBuilder() =>
                Host.CreateDefaultBuilder()
                    .ConfigureWebHostDefaults(webBuilder =>
                    {
                        webBuilder.Configure(app =>
                        {
                            app.Run(async context =>
                            {
                                var logger = context.RequestServices.GetRequiredService<ILoggerFactory>().CreateLogger("Test.Endpoint");
                                logger.LogInformation("Handled {path}", context.Request.Path.Value);
                                await context.Response.WriteAsync("ok");
                            });
                        });
                    });

            protected override void ConfigureWebHost(IWebHostBuilder builder)
            {
                builder.UseContentRoot(AppContext.BaseDirectory);
            }
        }
    }
}
