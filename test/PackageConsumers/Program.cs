extern alias xunit2;
extern alias xunit3;

using MELT;
using Microsoft.Extensions.Logging;
#if HAS_ASPNETCORE
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
#endif

using var factory = TestLoggerFactory.Create(logging => logging.AddSerilogTest());
factory.CreateLogger("Consumer").LogInformation("A packaged MELT consumer is running.");

if (!factory.Sink.LogEntries.Any())
{
    throw new InvalidOperationException("The packaged MELT logger did not capture a log entry.");
}

xunit2::Xunit.LoggingAssert.Contains("key", "value", new[] { new KeyValuePair<string, object>("key", "value") });
xunit3::Xunit.LoggingAssert.Contains("key", "value", new[] { new KeyValuePair<string, object>("key", "value") });

#if HAS_ASPNETCORE
WebApplication.CreateBuilder().WebHost.UseTestLogging().UseSerilogTestLogging();
#endif
