extern alias xunit2;
extern alias xunit3;

using MELT;
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

// 1. Core TestLoggerFactory, scopes, and Clear()
using (var factory = TestLoggerFactory.Create())
{
    var logger = factory.CreateLogger("Consumer.Core");
    using (logger.BeginScope("CoreScope"))
    {
        logger.LogInformation("A packaged MELT consumer is running for {target}.", "core");
    }

    var entry = factory.Sink.LogEntries.SingleOrDefault()
        ?? throw new InvalidOperationException("The packaged MELT logger did not capture a log entry.");
    if (entry.OriginalFormat != "A packaged MELT consumer is running for {target}.")
    {
        throw new InvalidOperationException($"Unexpected OriginalFormat: {entry.OriginalFormat}");
    }

    var scope = factory.Sink.Scopes.SingleOrDefault()
        ?? throw new InvalidOperationException("The packaged MELT logger did not capture a scope.");
    if (scope.Message != "CoreScope")
    {
        throw new InvalidOperationException($"Unexpected scope message: {scope.Message}");
    }

    // 2. xUnit v2 and xUnit v3 LoggingAssert
    xunit2::Xunit.LoggingAssert.Contains("target", "core", entry.Properties);
    xunit3::Xunit.LoggingAssert.Contains("target", "core", entry.Properties);

    factory.Sink.Clear();
    if (factory.Sink.LogEntries.Any() || factory.Sink.Scopes.Any())
    {
        throw new InvalidOperationException("ITestLoggerSink.Clear() did not clear captured entries and scopes.");
    }
}

// 3. Direct ISerilogTestLoggerSink verification via ServiceCollection + AddSerilogTest()
{
    var providers = new LoggerProviderCollection();
    var serilogLogger = new LoggerConfiguration()
        .MinimumLevel.Verbose()
        .Enrich.FromLogContext()
        .WriteTo.Providers(providers)
        .CreateLogger();

    try
    {
        var services = new ServiceCollection();
        services.AddLogging(logging => logging
            .ClearProviders()
            .AddSerilogTest(options => options.FilterByLoggerName("Consumer.SerilogDirect")));

        using var serviceProvider = services.BuildServiceProvider();
        foreach (var loggerProvider in serviceProvider.GetServices<ILoggerProvider>())
        {
            providers.AddProvider(loggerProvider);
        }

        using ILoggerFactory loggerFactory = new SerilogLoggerFactory(serilogLogger, dispose: false, providers);
        var logger = loggerFactory.CreateLogger("Consumer.SerilogDirect");
        using (logger.BeginScope("SerilogDirectScope"))
        {
            logger.LogInformation("Hello {place}!", "Serilog");
        }

        var serilogSink = serviceProvider.GetRequiredService<ISerilogTestLoggerSink>();
        var serilogEntry = serilogSink.LogEntries.SingleOrDefault()
            ?? throw new InvalidOperationException("ISerilogTestLoggerSink did not capture a log entry.");
        if (serilogEntry.Message != "Hello \"Serilog\"!")
        {
            throw new InvalidOperationException($"Unexpected Serilog message: {serilogEntry.Message}");
        }

        var serilogScope = serilogEntry.Scopes.SingleOrDefault() as ScalarValue;
        if (!Equals(serilogScope?.Value, "SerilogDirectScope"))
        {
            throw new InvalidOperationException($"Unexpected Serilog scope: {serilogScope}");
        }

        xunit2::Xunit.LoggingAssert.Contains("place", "Serilog", serilogEntry.Properties);
        xunit3::Xunit.LoggingAssert.Contains("place", "Serilog", serilogEntry.Properties);

        serilogSink.Clear();
        if (serilogSink.LogEntries.Any())
        {
            throw new InvalidOperationException("ISerilogTestLoggerSink.Clear() did not clear captured entries.");
        }
    }
    finally
    {
        serilogLogger.Dispose();
    }
}

// 4. ASP.NET Core helper (MELT.AspNetCore) end-to-end capture
using (var baseFactory = new StandardTestWebApplicationFactory())
using (var factory = baseFactory.WithWebHostBuilder(builder =>
    builder.UseTestLogging(options => options.FilterByLoggerName("Consumer.AspNetCore"))))
{
    using var client = factory.CreateDefaultClient();
    using var response = await client.GetAsync("/");
    response.EnsureSuccessStatusCode();

    var sink = factory.GetTestLoggerSink();
    if (!factory.TryGetTestLoggerSink(out var trySink) || !ReferenceEquals(sink, trySink))
    {
        throw new InvalidOperationException("MELT.AspNetCore TryGetTestLoggerSink did not return the registered sink.");
    }

    var entry = sink.LogEntries.SingleOrDefault()
        ?? throw new InvalidOperationException("MELT.AspNetCore did not capture an HTTP request log entry.");
    if (entry.Message != "Handled /")
    {
        throw new InvalidOperationException($"Unexpected ASP.NET Core log message: {entry.Message}");
    }

    var scope = sink.Scopes.SingleOrDefault()
        ?? throw new InvalidOperationException("MELT.AspNetCore did not capture an HTTP request scope.");
    if (scope.Message != "AspNetCoreScope")
    {
        throw new InvalidOperationException($"Unexpected ASP.NET Core scope: {scope.Message}");
    }
}

// 5. Serilog ASP.NET Core helper (MELT.Serilog.AspNetCore) end-to-end capture
{
    var providers = new LoggerProviderCollection();
    Log.Logger = new LoggerConfiguration()
        .MinimumLevel.Verbose()
        .Enrich.FromLogContext()
        .WriteTo.Providers(providers)
        .CreateLogger();

    try
    {
        using var baseFactory = new SerilogTestWebApplicationFactory(providers);
        using var factory = baseFactory.WithWebHostBuilder(builder =>
            builder.UseSerilogTestLogging(options => options.FilterByLoggerName("Consumer.SerilogAspNetCore")));

        using var client = factory.CreateDefaultClient();
        using var response = await client.GetAsync("/serilog");
        response.EnsureSuccessStatusCode();

        var sink = factory.GetSerilogTestLoggerSink();
        if (!factory.TryGetSerilogTestLoggerSink(out var trySerilogSink) || trySerilogSink is null)
        {
            throw new InvalidOperationException("MELT.Serilog.AspNetCore TryGetSerilogTestLoggerSink did not return a sink.");
        }

        var entry = sink.LogEntries.SingleOrDefault()
            ?? throw new InvalidOperationException("MELT.Serilog.AspNetCore did not capture an HTTP request log entry.");
        if (!trySerilogSink.LogEntries.Any())
        {
            throw new InvalidOperationException("MELT.Serilog.AspNetCore TryGetSerilogTestLoggerSink did not expose captured entries.");
        }
        if (entry.Message != "Handled \"/serilog\"")
        {
            throw new InvalidOperationException($"Unexpected Serilog ASP.NET Core log message: {entry.Message}");
        }

        var scope = entry.Scopes.SingleOrDefault() as ScalarValue;
        if (!Equals(scope?.Value, "SerilogAspNetCoreScope"))
        {
            throw new InvalidOperationException($"Unexpected Serilog ASP.NET Core scope: {scope}");
        }
    }
    finally
    {
        Log.CloseAndFlush();
    }
}

Console.WriteLine("All packed MELT consumer checks passed.");

internal sealed class ConsumerAppMarker
{
}

internal sealed class StandardTestWebApplicationFactory : WebApplicationFactory<ConsumerAppMarker>
{
    protected override IHostBuilder CreateHostBuilder() =>
        Host.CreateDefaultBuilder()
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.Configure(app =>
                {
                    app.Run(async context =>
                    {
                        var logger = context.RequestServices.GetRequiredService<ILoggerFactory>().CreateLogger("Consumer.AspNetCore");
                        using (logger.BeginScope("AspNetCoreScope"))
                        {
                            logger.LogInformation("Handled {path}", context.Request.Path.Value);
                            await context.Response.WriteAsync("ok");
                        }
                    });
                });
            });

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseContentRoot(AppContext.BaseDirectory);
    }
}

internal sealed class SerilogTestWebApplicationFactory : WebApplicationFactory<ConsumerAppMarker>
{
    private readonly LoggerProviderCollection _providers;

    public SerilogTestWebApplicationFactory(LoggerProviderCollection providers)
    {
        _providers = providers;
    }

    protected override IHostBuilder CreateHostBuilder() =>
        Host.CreateDefaultBuilder()
            .UseSerilog(providers: _providers)
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.Configure(app =>
                {
                    app.Run(async context =>
                    {
                        var logger = context.RequestServices.GetRequiredService<ILoggerFactory>().CreateLogger("Consumer.SerilogAspNetCore");
                        using (logger.BeginScope("SerilogAspNetCoreScope"))
                        {
                            logger.LogInformation("Handled {path}", context.Request.Path.Value);
                            await context.Response.WriteAsync("ok");
                        }
                    });
                });
            });

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseContentRoot(AppContext.BaseDirectory);
    }
}
