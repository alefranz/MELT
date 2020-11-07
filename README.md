# MELT

<!-- markdownlint-disable no-inline-html -->
<img align="right" width="128" height="128" src="logo.png">
<!-- markdownlint-enable no-inline-html -->

_Testing Library for Microsoft Extensions Logging._

[![Build Status](https://github.com/alefranz/MELT/workflows/Build/badge.svg?branch=main)](https://github.com/alefranz/MELT/actions?query=branch%3Amain)
[![MELT Nuget](https://img.shields.io/nuget/v/MELT?label=MELT&logo=nuget)](https://www.nuget.org/packages/MELT/)
[![MELT.AspNetCore Nuget](https://img.shields.io/nuget/v/MELT.AspNetCore?label=MELT.AspNetCore&logo=nuget)](https://www.nuget.org/packages/MELT.AspNetCore/)
[![MELT.Serilog Nuget](https://img.shields.io/nuget/v/MELT.Serilog?label=MELT.Serilog&logo=nuget)](https://www.nuget.org/packages/MELT.Serilog/)
[![MELT.Serilog.AspNetCore Nuget](https://img.shields.io/nuget/v/MELT.Serilog.AspNetCore?label=MELT.Serilog.AspNetCore&logo=nuget)](https://www.nuget.org/packages/MELT.Serilog.AspNetCore/)
[![MELT.Xunit Nuget](https://img.shields.io/nuget/v/MELT.Xunit?label=MELT.Xunit&logo=nuget)](https://www.nuget.org/packages/MELT.Xunit/)

<!-- omit in toc -->
## About MELT

MELT is a free, open-source, testing library for the .NET Standard _Microsoft Extensions Logging_ library.
It is a solution to easily test logs.

It is a repackaging with a sweetened API and some omissions of [Microsoft.Extensions.Logging.Testing](https://github.com/aspnet/Extensions/tree/master/src/Logging/Logging.Testing), a library used internally in [ASP.NET Core](https://github.com/aspnet/AspNetCore) for testing the logging, given that [there is currently no plan to offer an official package for it](https://github.com/aspnet/Extensions/issues/672#issuecomment-532850535).

It is licensed under [Apache License 2.0](https://github.com/alefranz/MELT/blob/main/LICENSE).
Most of the code is copyrighted by the .NET Foundation as mentioned in the files headers.

If you like this project please don't forget to *star* it on [GitHub](https//github.com/alefranz/MELT) or let me know with a [tweet](https://twitter.com/AleFranz).

You can find an explanation on the advantages of using this library and the importance of testing logs on the blog post "[How to test logging when using Microsoft.Extensions.Logging](https://alessio.franceschelli.me/posts/dotnet/how-to-test-logging-when-using-microsoft-extensions-logging/)".

> If you are upgrading to version 0.5, you can _optionally_ migrate to the new syntax. See [Upgrade from 0.4 and below](#upgrade-from-04-and-below) for more info.

<!-- omit in toc -->
## Index

- [Quickstart](#quickstart)
- [Assertions](#assertions)
  - [Assert log entries](#assert-log-entries)
  - [Assert scopes](#assert-scopes)
  - [Assert log original format](#assert-log-original-format)
  - [Assert exceptions in log entries](#assert-exceptions-in-log-entries)
  - [Easily test log or scope properties with xUnit](#easily-test-log-or-scope-properties-with-xunit)
  - [And much more](#and-much-more)
  - [Full example](#full-example)
- [Quickstart for ASP.NET Core integration tests](#quickstart-for-aspnet-core-integration-tests)
  - [Assert log entries and scopes](#assert-log-entries-and-scopes)
  - [Full example](#full-example-1)
- [Compatibility](#compatibility)
- [Serilog compatibility using Serilog.Extensions.Logging](#serilog-compatibility-using-serilogextensionslogging)
  - [Full example](#full-example-2)
- [Serilog compatibility using Serilog.AspNetCore](#serilog-compatibility-using-serilogaspnetcore)
  - [Assert log entries](#assert-log-entries-1)
  - [Assert scopes on an entry](#assert-scopes-on-an-entry)
  - [Assert message format](#assert-message-format)
  - [Easily test log or scope properties with xUnit](#easily-test-log-or-scope-properties-with-xunit-1)
  - [And much more](#and-much-more-1)
  - [Full example](#full-example-3)
- [NLog compatibility using NLog.Web.AspNetCore](#nlog-compatibility-using-nlogwebaspnetcore)
  - [Full example](#full-example-4)
- [Upgrade from 0.4 and below](#upgrade-from-04-and-below)
  - [Upgrade of ASP.NET Core Integration Tests](#upgrade-of-aspnet-core-integration-tests)

## Quickstart

- Install the NuGet package [MELT](https://www.nuget.org/packages/MELT/)

    ```xml
    <PackageReference Include="MELT" Version="0.6.0" />
    ```

    > Note: due to a breaking change in `Microsoft.Extensions.Logging` 3.1, if you are testing a project that references **only** `Microsoft.Extensions.Logging.Abstractions` 3.1, you need to add a reference to `Microsoft.Extensions.Logging` 3.1 in your test project:
    >
    > ```xml
    > <PackageReference Include="Microsoft.Extensions.Logging" Version="3.1.0" />
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
Each entry exposes all the relevant property for a log.

For example, to test with xUnit that a single log has been emitted and it had a specific message:

```csharp
var log = Assert.Single(loggerFactory.Sink.LogEntries);
Assert.Equal("The answer is 42", log.Message);
```

### Assert scopes

The logger factory exposes a property `Sink` to access the sink that collects the logs. The sink exposes a property `Scopes` that enumerates all the captured scopes.

For example, to test with xUnit that a single scope has been emitted and it had a specific message:

```csharp
var scope = Assert.Single(loggerFactory.Sink.Scopes);
Assert.Equal("I'm in the GET scope", scope.Message);
```

There is also a property `Scope` in each log entry to have the scope captured with that entry, however this only support the most inner scope and doesn't track it across different loggers.

```csharp
var log = Assert.Single(loggerFactory.Sink.LogEntries);
Assert.Equal("This scope's answer is 42", log.Scope.Message);
```

### Assert log original format

The original format used to generate the log entry, before the message is rendered, is captured in the property `OriginalMessage`.

```csharp
var log = Assert.Single(loggerFactory.Sink.LogEntries);
Assert.Equal("The answer is {number}", log.OriginalFormat);
```

### Assert exceptions in log entries

The log entry exposes a property `Exception` which contains the exception captured by the logger.

For example, to test with xUnit that a single log has been emitted with a specific exception and assert a exception property:

```csharp
var log = Assert.Single(loggerFactory.Sink.LogEntries);
var exception = Assert.IsType<ArgumentNullException>(log.Exception);
Assert.Equal("foo", exception.ParamName);
```

### Easily test log or scope properties with xUnit

- Install the NuGet package [MELT.Xunit](https://www.nuget.org/packages/MELT.Xunit/)

    ```xml
    <PackageReference Include="MELT.Xunit" Version="0.6.0" />
    ```

- Use the `LoggingAssert.Contains(...)` helpers.
For example, to test that a single log has been emitted and it had a property `number` with value `42`:

    ```csharp
    var log = Assert.Single(loggerFactory.Sink.LogEntries);
    LoggingAssert.Contains("number", 42, log.Properties);
    ```

### And much more

You can assert againt all the characteristic of a log entry: `EventId`, `Exception`, `LoggerName`, `LogLevel`, `Message`, `OriginalFormat`, `Properties` and `Scope`.

### Full example

See [SampleTest](samples/SampleLibrary.Tests/SampleTest.cs) and [MoreTest](samples/SampleLibrary.Tests/MoreTest.cs)

## Quickstart for ASP.NET Core integration tests

- Install the NuGet package [MELT.AspNetCore](https://www.nuget.org/packages/MELT.AspNetCore/)

    ```xml
    <PackageReference Include="MELT.AspNetCore" Version="0.6.0" />
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
            // We can clear the sink before each test execution, as xUnit will not run this tests in parallel.
            _factory.GetTestLoggerSink().Clear();
            // When running on 2.x, the server is not initialized until it is explicitly started or the first client is created.
            // So we need to use:
            // if (_factory.TryGetTestLoggerSink(out var testLoggerSink)) testLoggerSink.Clear();
        }
    }
    ```

    The logger will be automatically injected with Dependency Injection.

### Assert log entries and scopes

Once you access the `sink` with `_factory.GetTestLoggerSink()` you get access to the property `LogEntries` that enumerates all the captured logs and `Scopes` which enumerated all the captured `Scopes`.
You will then be able to do all the assertions like described above in [Assertions](#assertions)

For example, to test with xUnit that a single log has been emitted and it had a specific message:

```csharp
var log = Assert.Single(_factory.GetTestLoggerSink().LogEntries);
Assert.Equal("The answer is 42", log.Message);
```

### Full example

See [LoggingTest](samples/current/SampleWebApplication.IntegrationTests/LoggingTest.cs) or
[LoggingTestWithInjectedFactory](samples/current/SampleWebApplication.IntegrationTests/LoggingTestWithInjectedFactory.cs).

## Compatibility

This library is compatibe with [Microsoft.Extensions.Logging](https://github.com/aspnet/Extensions/tree/master/src/Logging/Logging.Testing) 2.0+.
When used for integration tests of ASP.NET Core applications, it supports all the currently supported versions of ASP.NET Core: 2.1 LTS and 3.1 LTS, but also the now deprecated 2.2 and 3.0.

## Serilog compatibility using Serilog.Extensions.Logging

If you are using [Serilog.Extensions.Logging](https://github.com/serilog/serilog-extensions-logging) the integration is straightforward as this library is fully compliant with `Microsoft.Extensions.Logging`.

Simply follow the main instruction as the fact that you are plugging Serilog as the provider does not alter the behaviour.

### Full example

See [LoggingTest](samples/current/serilog/SampleWebApplicationSerilog.IntegrationTests/LoggingTest.cs) or
[LoggingTestWithInjectedFactory](samples/current/serilog/SampleWebApplicationSerilog.IntegrationTests/LoggingTestWithInjectedFactory.cs).

## Serilog compatibility using Serilog.AspNetCore

Unfortunately, [Serilog.AspNetCore](https://github.com/serilog/serilog-aspnetcore) doesn't plug nicely into `Microsoft.Extensions.Logging` as it replaces the logger factory and brings in an opinionated behaviour.

However, `MELT` has specific support to allow to write tests against the Serilog produced logs, also allowing you to verify the Serilog behaviours (e.g. object expansion).

- Modify your `Program.cs` of your ASP.NET Core applications, defining a `LoggerProviderCollection` to be able to hook into the logging from the tests later on. Then, pass it to the `UseSerilog()` extension method of the web host builder.

    ```csharp
    public class Program
    {
        public static readonly LoggerProviderCollection Providers = new LoggerProviderCollection();  // <---

        public static int Main(string[] args)
        {
            // ...
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder
                        // ...
                        .UseSerilog(providers: Providers);  // <---
                });
        }
    }
    ```

- Now go back to your integration tests project, and install the NuGet package [MELT.Serilog.AspNetCore](https://www.nuget.org/packages/MELT.Serilog.AspNetCore/)

    ```xml
    <PackageReference Include="MELT.Serilog.AspNetCore" Version="0.6.0" />
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
Each entry exposes all the relevant property for a log.

For example, to test with xUnit that a single log has been emitted and it had a specific message:

```csharp
var log = Assert.Single(_factory.GetSerilogTestLoggerSink().LogEntries);
Assert.Equal("Hello \"World\"!", log.Message);
```

Please note that Serilog adds double quotes around parameters.

### Assert scopes on an entry

The log entry exposes a property `Scopes` that enumerates all the scopes captured for that log entry.

For example, to test with xUnit that a single scope has been emitted and it had a specific message:

```csharp
var log = Assert.Single(_factory.GetSerilogTestLoggerSink().LogEntries);
var scope = Assert.Single(log.Scope);
Assert.Equal(new ScalarValue("I'm in the GET scope"), scope);
```

The scope is preserved in the Serilog format, so you can use the Serilog `DictionaryValue`, `ScalarValue`, `SequenceValue` or `StructureValue`.

If you have multiple nested scopes, you can assert with:

```csharp
Assert.Collection(log.Scope,
    x => Assert.Equal(new ScalarValue("A top level scope"), x),
    x => Assert.Equal(new ScalarValue("I'm in the GET scope"), x)
);
```

### Assert message format

```csharp
var log = Assert.Single(loggerFactory.LogEntries);
Assert.Equal("The answer is {number}", log.OriginalFormat);
```

### Easily test log or scope properties with xUnit

- Install the NuGet package [MELT.Xunit](https://www.nuget.org/packages/MELT.Xunit/)

    ```xml
    <PackageReference Include="MELT.Xunit" Version="0.6.0" />
    ```

- Use the `LoggingAssert.Contains(...)` helpers.
    For example, to test that a single log has been emitted and it had a property `number` with value `42`:

    ```csharp
    var log = Assert.Single(_factory.GetSerilogTestLoggerSink().LogEntries);
    LoggingAssert.Contains("place", "World", log.Properties);
    ```

    Note that if you have added to the scope a dictionary, Serilog will only add the properties to the log entry it self, without create a scope:

    ```csharp
    Assert.Empty(log.Scope);
    LoggingAssert.Contains("foo", "bar", log.Properties);
    LoggingAssert.Contains("answer", 42, log.Properties);
    ```

### And much more

You can assert againt all the characteristic of a log entry: `EventId`, `Exception`, `LoggerName`, `LogLevel`, `Message`, `Properties`, `OriginalFormat` and `Scope`.

### Full example

See [LoggingTest](samples/current/serilog/SampleWebApplicationSerilogAlternate.IntegrationTests/LoggingTest.cs) or
[LoggingTestWithInjectedFactory](samples/current/serilog/SampleWebApplicationSerilogAlternate.IntegrationTests/LoggingTestWithInjectedFactory.cs).

## NLog compatibility using NLog.Web.AspNetCore

If you are using [NLog.Web.AspNetCore](https://github.com/NLog/NLog.Web) the integration is straightforward as this library is fully compliant with `Microsoft.Extensions.Logging`.

Simply follow the main instruction as the fact that you are plugging NLog as the provider does not alter the behaviour.

### Full example

See [LoggingTest](samples/current/NLog/SampleWebApplicationNLog.IntegrationTests/LoggingTest.cs) or
[LoggingTestWithInjectedFactory](samples/current/NLog/SampleWebApplicationNLog.IntegrationTests/LoggingTestWithInjectedFactory.cs).

## Upgrade from 0.4 and below

The library is still backward compatible, however, if you follow the deprecation warnings, you will be able to easily migrate to the new simplified syntax.

> Note: due to a breaking change in `Microsoft.Extensions.Logging` 3.1, if you are testing a project that references **only** `Microsoft.Extensions.Logging.Abstractions` 3.1, you need to add a reference to `Microsoft.Extensions.Logging` 3.1 in your test project:
>
> ```xml
> <PackageReference Include="Microsoft.Extensions.Logging" Version="3.1.0" />
> ```

Here are some common examples:

Setting up logger factory for tests.

```csharp
var loggerFactory = MELTBuilder.CreateLoggerFactory();
// become
var loggerFactory = TestLoggerFactory.Create();
```

Setting up logger factory for tests, filtering messages by component.

```csharp
var loggerFactory = MELTBuilder.CreateLoggerFactory(options => options.FilterByTypeName<MyClass>());
// become
var loggerFactory = TestLoggerFactory.Create(options => options.FilterByTypeName<MyClass>());
```

Accessing captured logs from the factory

```csharp
var log = Assert.Single(loggerFactory.LogEntries);
// become
var log = Assert.Single(loggerFactory.Sink.LogEntries);
```

Accessing the original format of a log entry

```csharp
Assert.Equal("More is less.", log.Format);
// become
Assert.Equal("More is less.", log.OriginalFormat);
```

Assert against log properties

```csharp
LogValuesAssert.Contains("number", 42, log);
// become
LoggingAssert.Contains("number", 42, log.Properties);
```

Assert against scope properties

```csharp
LogValuesAssert.Contains("number", 42, scope);
// become
LoggingAssert.Contains("number", 42, scope.Properties);
```

And to assert log properties, the `using` is no longer needed

```csharp
using MELT.Xunit;
// no longer needed :)
```

### Upgrade of ASP.NET Core Integration Tests

Setting up the web application factory with the test logger

```csharp
_sink = MELTBuilder.CreateTestSink(options => options.FilterByNamespace(nameof(SampleWebApplication)));
_factory = factory.WithWebHostBuilder(builder => builder.ConfigureLogging(logging => logging.AddTestLogger(_sink)));
// become
_factory = factory.WithWebHostBuilder(builder => builder.UseTestLogging(options => options.FilterByNamespace(nameof(SampleWebApplication))));
```

Accessing the captured logs

```csharp
var log = Assert.Single(_sink.LogEntries);
// become
var log = Assert.Single(_factory.GetTestLoggerSink().LogEntries);
```
