# MELT

_Testing Library for Microsoft Extensions Logging._

<!-- omit in toc -->
## About MELT

MELT is a free, open-source, testing library for the .NET Standard _Microsoft Extensions Logging_ library.
It is a solution to easily test logs.

If you like this project please don't forget to **star** it on [GitHub](https://github.com/alefranz/MELT) or let me know with a [tweet](https://twitter.com/AleFranz).

You can find an explanation on the advantages of using this library and the importance of testing logs on the blog post "[How to test logging when using Microsoft.Extensions.Logging](https://alessio.franceschelli.me/posts/dotnet/how-to-test-logging-when-using-microsoft-extensions-logging/)".

For v2 upgrade guidance and compatibility details, see [Migrate to MELT
v2](MIGRATION-v2.md).

## Quickstart

- Install the NuGet package [MELT](https://www.nuget.org/packages/MELT/)

    ```xml
    <PackageReference Include="MELT" Version="2.0.0" />
    ```

    > Note: MELT v2 requires `Microsoft.Extensions.Logging` 8.0.0 or later. If your project pins `Microsoft.Extensions.Logging.Abstractions` separately, include a matching or newer `Microsoft.Extensions.Logging` reference in the test project:
    >
    > ```xml
    > <PackageReference Include="Microsoft.Extensions.Logging" Version="8.0.0" />
    > ```

- Get a test logger factory

    ```csharp
    var loggerFactory = TestLoggerFactory.Create();
    ```

- Get a logger from the factory, as usual, to pass to your fixture.

    ```csharp
    var logger = loggerFactory.CreateLogger<Sample>();
    ```

## Assertions

### Assert log entries

The logger factory exposes a property `Sink` to access the sink that collects the logs. The sink exposes a property `LogEntries` that enumerates all the captured logs.
Each entry exposes the relevant properties of a log.

For example, to test with xUnit that a single log has been emitted and it has a specific message:

```csharp
var log = Assert.Single(loggerFactory.Sink.LogEntries);
Assert.Equal("The answer is 42", log.Message);
```

### Assert scopes

Each log entry exposes a property `Scopes` to have the scopes active for the particular logger captured with that entry.
_Note that the scopes of a log entry are local to a specific logger and async context._

```csharp
var log = Assert.Single(loggerFactory.Sink.LogEntries);
var scope = Assert.Single(log.Scopes);
Assert.Equal("This scope's answer is 42", scope.Message);
```

It is also possible to get all the scopes generated across all loggers. The logger factory exposes a property `Sink` to access the sink that collects the logs. The sink exposes a property `Scopes` that enumerates all the captured scopes.

For example, to test with xUnit that a single scope was emitted with a specific message:

```csharp
var scope = Assert.Single(loggerFactory.Sink.Scopes);
Assert.Equal("I'm in the GET scope", scope.Message);
```

### Assert log original format

The original format used to generate the log entry, before the message is rendered, is captured in the property `OriginalMessage`.

```csharp
var log = Assert.Single(loggerFactory.Sink.LogEntries);
Assert.Equal("The answer is {number}", log.OriginalFormat);
```

### Assert exceptions in log entries

The log entry exposes a property `Exception` which contains the exception captured by the logger.

For example, to test with xUnit that a single log was emitted with a specific exception and assert an exception property:

```csharp
var log = Assert.Single(loggerFactory.Sink.LogEntries);
var exception = Assert.IsType<ArgumentNullException>(log.Exception);
Assert.Equal("foo", exception.ParamName);
```

### Easily test log or scope properties with xUnit.v3

- Install the NuGet package [MELT.Xunit.v3](https://www.nuget.org/packages/MELT.Xunit.v3/)

    ```xml
    <PackageReference Include="MELT.Xunit.v3" Version="2.0.0" />
    ```

- Use the `LoggingAssert.Contains(...)` helpers.
For example, to test that a single log was emitted with a property `number` whose value is `42`:

    ```csharp
    var log = Assert.Single(loggerFactory.Sink.LogEntries);
    LoggingAssert.Contains("number", 42, log.Properties);
    ```

### Easily test log or scope properties with xUnit v2

- Install the NuGet package [MELT.Xunit](https://www.nuget.org/packages/MELT.Xunit/)

    ```xml
    <PackageReference Include="MELT.Xunit" Version="2.0.0" />
    ```

- Use the `LoggingAssert.Contains(...)` helpers.
For example, to test that a single log was emitted with a property `number` whose value is `42`:

    ```csharp
    var log = Assert.Single(loggerFactory.Sink.LogEntries);
    LoggingAssert.Contains("number", 42, log.Properties);
    ```

### And much more

You can assert against every characteristic of a log entry: `EventId`,
`Exception`, `LoggerName`, `LogLevel`, `Message`, `OriginalFormat`,
`Properties`, and `Scopes`.

### Full example

See [Samples](https://github.com/alefranz/MELT/tree/main/samples)

## Quickstart for ASP.NET Core integration tests

- Install the NuGet package [MELT.AspNetCore](https://www.nuget.org/packages/MELT.AspNetCore/)

    ```xml
    <PackageReference Include="MELT.AspNetCore" Version="2.0.0" />
    ```

- Use the `UseTestLogging(...)` extension method to add a test logger to the test web host builder, where you can also customize the behaviour.

    For example to filter all log entries and scopes not generated by loggers consumed in the `SampleWebApplication.*` namespace (this filters the logger name so it assumes you are using `ILogger<T>` or following the default naming convention for your loggers.)

    This can be done where you are already configuring the web host builder. Configure the logger using `WithWebHostBuilder` on the factory.

    ```csharp
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Mvc.Testing;
    // ...
    _factory = factory.WithWebHostBuilder(builder => builder
        .UseTestLogging(options => options.FilterByNamespace(nameof(SampleWebApplication))));
    ```

    You can also filter by logger name using `FilterByTypeName<T>()` or `FilterByLoggerName(string name)`.

    Or, if you prefer, you can use the `AddTest(...)` extension method in the `ConfigureLogging(...)` section.

    ```csharp
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Mvc.Testing;
    using Microsoft.Extensions.Logging;
    // ...
    _factory = factory.WithWebHostBuilder(builder => builder
        .ConfigureLogging(logging => logging.AddTest(options => options.FilterByNamespace(nameof(SampleWebApplication)))));
    ```

- Alternatively, you can configure the logger builder in the `ConfigureWebHost` implementation in your custom `WebApplicationFactory<TStartup>`.

    ```csharp
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Mvc.Testing;

    namespace SampleWebApplication.IntegrationTests
    {
        public class CustomWebApplicationFactory<TStartup> : WebApplicationFactory<TStartup>
            where TStartup : class
        {
            protected override void ConfigureWebHost(IWebHostBuilder builder)
            {
                builder.UseTestLogging(options => options.FilterByNamespace(nameof(SampleWebApplication)));
            }
        }
    }
    ```

    You can then retrieve the sink to assert against using the extension method `GetTestLoggerSink()` on the factory.

    Please note that in this case, all tests sharing the same factory will get the same sink.
    You can reset it between tests with `Clear()` in the constructor of your `xUnit` tests. For example:

    ```csharp
    public class LoggingTestWithInjectedFactory : IClassFixture<CustomWebApplicationFactory<Startup>>
    {
        private readonly CustomWebApplicationFactory<Startup> _factory;

        public LoggingTestWithInjectedFactory(CustomWebApplicationFactory<Startup> factory)
        {
            _factory = factory;
            // In this case, the factory will be reused for all tests, so the sink will be shared as well.
            // We can clear the sink before each test execution because xUnit will not run these tests in parallel.
            _factory.GetTestLoggerSink().Clear();
            // When running on 2.x, the server is not initialized until it is explicitly started or the first client is created.
            // So we need to use:
            // if (_factory.TryGetTestLoggerSink(out var testLoggerSink)) testLoggerSink.Clear();
        }
    }
    ```

    The logger will be automatically injected with Dependency Injection.

### Assert log entries and scopes

Once you access the sink with `_factory.GetTestLoggerSink()`, `LogEntries`
enumerates captured logs and `Scopes` enumerates captured scopes. You can then
make the assertions described in [Assertions](#assertions).

For example, to test with xUnit that a single log was emitted with a specific message:

```csharp
var log = Assert.Single(_factory.GetTestLoggerSink().LogEntries);
Assert.Equal("The answer is 42", log.Message);
```

### Full example

See [LoggingTest](https://github.com/alefranz/MELT/blob/main/samples/current/SampleWebApplication.IntegrationTests/LoggingTest.cs) or
[LoggingTestWithInjectedFactory](https://github.com/alefranz/MELT/blob/main/samples/current/SampleWebApplication.IntegrationTests/LoggingTestWithInjectedFactory.cs).

## Compatibility

The `MELT`, `MELT.Serilog`, `MELT.Xunit`, and `MELT.Xunit.v3` packages target
`netstandard2.0` and require
[Microsoft.Extensions.Logging](https://github.com/dotnet/extensions/tree/main/src/Logging)
8.0.0 or later. They are actively supported for applications targeting .NET 8,
.NET 9, and .NET 10. .NET Framework 4.7.2 is best-effort compatibility for these core
packages and is not covered by CI.

The ASP.NET Core helper packages target .NET 8 and .NET 10. They use the
corresponding modern ASP.NET Core testing stacks; MELT v2 does not support the
legacy ASP.NET Core 2.1 helper dependencies.

### Sample runtime matrix

The core packages retain a `netstandard2.0` asset. Dedicated .NET 8 samples
validate the minimum supported dependency baseline, while the default samples
and core test suites run on .NET 10. The explicitly named
`SampleWebApplication.Net9` sample and its integration tests cover the .NET 9
web compatibility variant; they are not the default target.

### MELT v2 migration and release notes

MELT v2 is the current release line. Applications using a Microsoft.Extensions
dependency line earlier than 8.0, or legacy ASP.NET Core helpers, should remain
on MELT 1.x. See the [v2 migration guide](https://github.com/alefranz/MELT/blob/main/docs/MIGRATION-v2.md)
and [v2 release notes](https://github.com/alefranz/MELT/blob/main/docs/RELEASE-NOTES-v2.md)
for the breaking support-policy change.

## Serilog compatibility using Serilog.Extensions.Logging

If you are using [Serilog.Extensions.Logging](https://github.com/serilog/serilog-extensions-logging) the integration is straightforward as this library is fully compliant with `Microsoft.Extensions.Logging`.

Follow the main instructions: using Serilog as the provider does not alter MELT's behaviour.

The `SampleWebApplicationSerilog` sample pins the supported Serilog `3.1.1`
and Serilog.Extensions.Logging `8.0.0` minimum. The
`SampleWebApplicationSerilogAlternate` sample pins Serilog `4.4.0` to
validate the current major line.

### Full example

See [LoggingTest](https://github.com/alefranz/MELT/blob/main/samples/current/serilog/SampleWebApplicationSerilog.IntegrationTests/LoggingTest.cs) or
[LoggingTestWithInjectedFactory](https://github.com/alefranz/MELT/blob/main/samples/current/serilog/SampleWebApplicationSerilog.IntegrationTests/LoggingTestWithInjectedFactory.cs).

## Serilog compatibility using Serilog.AspNetCore

[Serilog.AspNetCore](https://github.com/serilog/serilog-aspnetcore) replaces the logger factory and has opinionated behaviour, so it requires a different setup.

However, `MELT` has specific support for testing logs produced by Serilog,
including Serilog-specific behaviour such as object expansion.

- Modify your `Program.cs` of your ASP.NET Core applications, defining a `LoggerProviderCollection` to be able to hook into the logging from the tests later on. Then, pass it to the `UseSerilog()` extension method of the web host builder.

    ```csharp
    public class Program
    {
        public static readonly LoggerProviderCollection Providers = new LoggerProviderCollection();  // <---

        public static int Main(string[] args)
        {
            // ...
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseSerilog(providers: Providers)  // <---
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
    ```

- Now go back to your integration tests project, and install the NuGet package [MELT.Serilog.AspNetCore](https://www.nuget.org/packages/MELT.Serilog.AspNetCore/)

    ```xml
    <PackageReference Include="MELT.Serilog.AspNetCore" Version="2.0.0" />
    ```

- Define a Serilog logger, setting it up to write to the providers' collection we had previously added to `Program.cs`

    ```csharp
    Log.Logger = new LoggerConfiguration()
        .MinimumLevel.Verbose()
        .Enrich.FromLogContext()
        .WriteTo.Providers(Program.Providers)  // <---
        .CreateLogger();
    ```

- Use the `UseSerilogTestLogging(...)` extension method to add a test logger to the test web host builder, where you can also customize the behaviour.

    For example to filter all log entries and scopes not generated by loggers consumed in the `SampleWebApplication.*` namespace (this filters the logger name so it assumes you are using `ILogger<T>` or following the default naming convention for your loggers.)

    This can be done where you are already configuring the web host builder. Configure the logger using `WithWebHostBuilder` on the factory.

    ```csharp
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Mvc.Testing;
    // ...
    _factory = factory.WithWebHostBuilder(builder => builder
        .UseSerilogTestLogging(options => options.FilterByNamespace(nameof(SampleWebApplicationSerilogAlternate))));
    ```

    You can also filter by logger name using `FilterByTypeName<T>()` or `FilterByLoggerName(string name)`.

    Or, if you prefer, you can use the `AddSerilogTest(...)` extension method in the `ConfigureLogging(...)` section.

    ```csharp
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Mvc.Testing;
    using Microsoft.Extensions.Logging;
    // ...
    _factory = factory.WithWebHostBuilder(builder => builder
        .ConfigureLogging(logging => logging.AddSerilogTest(options => options.FilterByNamespace(nameof(SampleWebApplicationSerilogAlternate)))));
    ```

- Alternatively, you can configure the logger builder in the `ConfigureWebHost` implementation in your custom `WebApplicationFactory<TStartup>`.

    ```csharp
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Mvc.Testing;
    using Serilog;

    namespace SampleWebApplicationSerilogAlternate.IntegrationTests
    {
        public class CustomWebApplicationFactory<TStartup> : WebApplicationFactory<TStartup>
            where TStartup : class
        {
            public CustomWebApplicationFactory()
            {
                Log.Logger = new LoggerConfiguration()
                    .MinimumLevel.Verbose()
                    .Enrich.FromLogContext()
                    .WriteTo.Providers(Program.Providers)
                    .CreateLogger();
            }

            protected override void ConfigureWebHost(IWebHostBuilder builder)
            {
                builder.UseSerilogTestLogging(options =>
                {
                    options.FilterByNamespace(nameof(SampleWebApplicationSerilogAlternate));
                });
            }

            protected override void Dispose(bool disposing)
            {
                if (true)
                {
                    Log.CloseAndFlush();
                }
            }
        }
    }
    ```

    You can then retrieve the sink to assert against using the extension method `GetSerilogTestLoggerSink()` on the factory.

    Please note that in this case, all tests sharing the same factory will get the same sink.
    You can reset it between tests with `Clear()` in the constructor of your `xUnit` tests. For example:

    ```csharp
    public class LoggingTestWithInjectedFactory : IClassFixture<CustomWebApplicationFactory<Startup>>
    {
        private readonly CustomWebApplicationFactory<Startup> _factory;

        public LoggingTestWithInjectedFactory(CustomWebApplicationFactory<Startup> factory)
        {
            _factory = factory;
            // In this case, the factory will be reused for all tests, so the sink will be shared as well.
            // We can clear the sink before each test execution, as xUnit will not run this tests in parallel.
            _factory.GetSerilogTestLoggerSink().Clear();
            // When running on 2.x, the server is not initialized until it is explicitly started or the first client is created.
            // So we need to use:
            // if (_factory.TryGetSerilogTestLoggerSink(out var testLoggerSink)) testLoggerSink.Clear();
        }
    }
    ```

    The logger will be automatically injected with Dependency Injection.

### Assert log entries

The sink exposes a property `LogEntries` that enumerates all the logs captured.
Each entry exposes the relevant properties of a log.

For example, to test with xUnit that a single log was emitted with a specific message:

```csharp
var log = Assert.Single(_factory.GetSerilogTestLoggerSink().LogEntries);
Assert.Equal("Hello \"World\"!", log.Message);
```

Please note that Serilog adds double quotes around parameters.

### Assert scopes on an entry

The log entry exposes a property `Scopes` that enumerates all the scopes captured for that log entry.

For example, to test with xUnit that a single scope was emitted with a specific message:

```csharp
var log = Assert.Single(_factory.GetSerilogTestLoggerSink().LogEntries);
var scope = Assert.Single(log.Scopes);
Assert.Equal(new ScalarValue("I'm in the GET scope"), scope);
```

The scope is preserved in the Serilog format, so you can use the Serilog `DictionaryValue`, `ScalarValue`, `SequenceValue` or `StructureValue`.

If you have multiple nested scopes, you can assert with:

```csharp
Assert.Collection(log.Scopes,
    x => Assert.Equal(new ScalarValue("A top level scope"), x),
    x => Assert.Equal(new ScalarValue("I'm in the GET scope"), x)
);
```

### Assert message format

```csharp
var log = Assert.Single(_factory.GetSerilogTestLoggerSink().LogEntries);
Assert.Equal("The answer is {number}", log.OriginalFormat);
```

### Easily test log or scope properties with xUnit.v3

- Install the NuGet package [MELT.Xunit.v3](https://www.nuget.org/packages/MELT.Xunit.v3/)

    ```xml
    <PackageReference Include="MELT.Xunit.v3" Version="2.0.0" />
    ```

- Use the `LoggingAssert.Contains(...)` helpers.
    For example, to test that a single log was emitted with a property `number` whose value is `42`:

    ```csharp
    var log = Assert.Single(_factory.GetSerilogTestLoggerSink().LogEntries);
    LoggingAssert.Contains("place", "World", log.Properties);
    ```

    If a scope contains a dictionary, Serilog adds its properties to the log
    entry rather than creating a scope:

    ```csharp
    Assert.Empty(log.Scopes);
    LoggingAssert.Contains("foo", "bar", log.Properties);
    LoggingAssert.Contains("answer", 42, log.Properties);
    ```

### Easily test log or scope properties with xUnit v2

- Install the NuGet package [MELT.Xunit](https://www.nuget.org/packages/MELT.Xunit/)

    ```xml
    <PackageReference Include="MELT.Xunit" Version="2.0.0" />
    ```

- Use the `LoggingAssert.Contains(...)` helpers.
    For example, to test that a single log was emitted with a property `number` whose value is `42`:

    ```csharp
    var log = Assert.Single(_factory.GetSerilogTestLoggerSink().LogEntries);
    LoggingAssert.Contains("place", "World", log.Properties);
    ```

    If a scope contains a dictionary, Serilog adds its properties to the log
    entry rather than creating a scope:

    ```csharp
    Assert.Empty(log.Scopes);
    LoggingAssert.Contains("foo", "bar", log.Properties);
    LoggingAssert.Contains("answer", 42, log.Properties);
    ```

### And much more

You can assert against every characteristic of a log entry: `EventId`,
`Exception`, `LoggerName`, `LogLevel`, `Message`, `Properties`,
`OriginalFormat`, and `Scopes`.

### Full example

See [LoggingTest](https://github.com/alefranz/MELT/blob/main/samples/current/serilog/SampleWebApplicationSerilogAlternate.IntegrationTests/LoggingTest.cs) or
[LoggingTestWithInjectedFactory](https://github.com/alefranz/MELT/blob/main/samples/current/serilog/SampleWebApplicationSerilogAlternate.IntegrationTests/LoggingTestWithInjectedFactory.cs).

## NLog compatibility using NLog.Web.AspNetCore

If you are using [NLog.Web.AspNetCore](https://github.com/NLog/NLog.Web) the integration is straightforward as this library is fully compliant with `Microsoft.Extensions.Logging`.

Follow the main instructions: using NLog as the provider does not alter MELT's behaviour.

### Full example

See [LoggingTest](https://github.com/alefranz/MELT/blob/main/samples/current/NLog/SampleWebApplicationNLog.IntegrationTests/LoggingTest.cs) or
[LoggingTestWithInjectedFactory](https://github.com/alefranz/MELT/blob/main/samples/current/NLog/SampleWebApplicationNLog.IntegrationTests/LoggingTestWithInjectedFactory.cs).
