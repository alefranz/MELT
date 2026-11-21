# Migrate to MELT v2

MELT v2 is a breaking release focused on the currently supported .NET
platform and a streamlined logging capture API. It is the release line for
applications targeting .NET 10 or later and using
`Microsoft.Extensions.Logging` 10.0.0 or later (and `Serilog` 4.2.0 /
`Serilog.Extensions.Logging` 10.0.0 or later when using `MELT.Serilog`).

## Who should upgrade

Upgrade to v2 if your application targets .NET 10 or later (actively supported
and validated on .NET 10 and .NET 11) and can use the Microsoft.Extensions
10.0 dependency line. The core packages (`MELT`, `MELT.Serilog`, `MELT.Xunit`,
and `MELT.Xunit.v3`) still ship `netstandard2.0` assets: an unsupported runtime
(such as .NET 8, .NET 9, or .NET Framework 4.7.2+) is not necessarily
technically incompatible with these core packages when paired with
`Microsoft.Extensions.*` 10.0.0 or later, though it is outside MELT v2's
active support policy and CI coverage. By contrast, `MELT.AspNetCore` and
`MELT.Serilog.AspNetCore` target `net10.0` and strictly require .NET 10 or
later.

Stay on the MELT 1.x legacy release line (latest: 1.1.0) if your application
must remain on Microsoft.Extensions packages earlier than 10.0 (including .NET 8
or .NET 9 targets or classic .NET Framework applications pinned to older
Extensions packages), relies on Serilog 3.x, uses the legacy ASP.NET Core 2.1
helper dependencies, or requires obsolete MELT 1.x APIs that were removed in v2.

## Recommended two-stage migration

Separating API migration from framework and dependency upgrades makes test
failures easier to diagnose:

1. **Upgrade all MELT packages to the latest 1.x release (`1.1.0`).**
2. **Resolve every MELT obsolete warning on 1.x.** The replacement APIs
   (`TestLoggerFactory.Create`, `ITestLoggerFactory.Sink`, `ITestLoggerSink`,
   `AddTest`, `UseTestLogging`, `GetTestLoggerSink`, `TryGetTestLoggerSink`,
   `LogEntry.OriginalFormat`, `LogEntry.Scopes`, `SerilogLogEntry.Scopes`, and
   `Xunit.LoggingAssert`) are already available in 1.x. Temporarily treating
   obsolete warnings as errors (`<WarningsAsErrors>CS0612;CS0618</WarningsAsErrors>`)
   ensures no removed compatibility API remains. Also check for any calls to
   `new TestLogger(string, ITestSink)` or references to `TestLoggerProvider._sink`,
   as `ITestSink` is removed in v2.
3. **Verify the test suite still passes on 1.x.**
4. **Upgrade all MELT packages together to v2 and update framework and dependency baselines:**
   - Target .NET 10 or later for actively supported runtimes. For ASP.NET Core
     integration tests, `MELT.AspNetCore` and `MELT.Serilog.AspNetCore`
     directly target `net10.0` (requiring `Microsoft.AspNetCore.Mvc.Testing`
     10.0.0 or later) and strictly require .NET 10 or later. (The core
     `netstandard2.0` packages do not impose a TFM block on other
     `netstandard2.0`-compatible runtimes that can use `Microsoft.Extensions.*`
     10.0.0+, though only .NET 10 and .NET 11 are supported and CI-tested.)
   - Update direct `Microsoft.Extensions.*` package references in the test
     project to `10.0.0` or later. A project that separately references
     `Microsoft.Extensions.Logging.Abstractions` should also reference a
     matching or newer `Microsoft.Extensions.Logging` package.
   - If using `MELT.Serilog` or `MELT.Serilog.AspNetCore`, update `Serilog` to
     `4.2.0` or later and `Serilog.Extensions.Logging` to `10.0.0` or later.
   - If using `MELT.Serilog.AspNetCore`, rename any calls from
     `TryGetTestLoggerSink(out ISerilogTestLoggerSink?)` to
     `TryGetSerilogTestLoggerSink(out ISerilogTestLoggerSink?)`.
   - Run the test suite on v2.

## Removed or renamed APIs and replacements

All APIs marked `[Obsolete]` in MELT 1.x—along with `TestLogger(string, ITestSink)`,
whose `ITestSink` parameter type was removed—have been removed in MELT v2, and
the Serilog `TryGetTestLoggerSink` extension has been renamed for symmetry with
`GetSerilogTestLoggerSink`:

| Removed or renamed API in v2 | Replacement |
| --- | --- |
| `MELT.ITestSink` | `MELT.ITestLoggerSink` |
| `MELT.IInternalTestSink` | Removed (internal capture store; use `ITestLoggerSink`) |
| `MELT.TestSink` | `TestLoggerFactory.Create(...)`, `ILoggingBuilder.AddTest(...)`, or `new TestLoggerProvider()` (`provider.Sink`) |
| `MELT.WriteContext` | `MELT.LogEntry` (read from `ITestLoggerSink.LogEntries`) |
| `WriteContext.Scope` | `LogEntry.Scopes` |
| `MELT.BeginScopeContext` | `MELT.BeginScope` (read from `ITestLoggerSink.Scopes`) |
| `MELT.MELTBuilder` (`CreateTestSink`, `CreateLoggerFactory`) | `TestLoggerFactory.Create(...)`, `IWebHostBuilder.UseTestLogging(...)`, and `WebApplicationFactory<TStartup>.GetTestLoggerSink()` |
| `TestLogger(string, ITestSink)` *(non-obsolete in 1.x)* | `TestLoggerFactory.Create(...)` or `new TestLoggerProvider().CreateLogger(string)` |
| `TestLoggerFactory(ITestSink)` | `TestLoggerFactory.Create(...)` |
| `TestLoggerFactory.LogEntries` / `ITestLoggerFactory.LogEntries` | `ITestLoggerFactory.Sink.LogEntries` |
| `TestLoggerFactory.Scopes` / `ITestLoggerFactory.Scopes` | `ITestLoggerFactory.Sink.Scopes` |
| `TestLoggerProvider._sink` | `ITestLoggerProvider.Sink` (`TestLoggerProvider.Sink`) |
| `TestLoggerProvider(ITestSink)` | `new TestLoggerProvider()` or `ILoggingBuilder.AddTest(...)` |
| `MELTLoggingBuilderExtensions.AddTestLogger(...)` | `MELTLoggingBuilderExtensions.AddTest(...)` |
| `MELTWebHostBuilderExtensions.UseTestLogging(IWebHostBuilder, ITestSink)` | `MELTWebHostBuilderExtensions.UseTestLogging(IWebHostBuilder)` / `UseTestLogging(IWebHostBuilder, Action<TestLoggerOptions>)` and `GetTestLoggerSink()` |
| `MELTWebApplicationFactoryExtensions.TryGetTestSink(...)` | `MELTWebApplicationFactoryExtensions.TryGetTestLoggerSink(...)` |
| `MELTWebApplicationFactoryExtensions.GetTestSink(...)` | `MELTWebApplicationFactoryExtensions.GetTestLoggerSink(...)` |
| `MELTSerilogWebApplicationFactoryExtensions.TryGetTestLoggerSink(...)` *(renamed in v2)* | `MELTSerilogWebApplicationFactoryExtensions.TryGetSerilogTestLoggerSink(...)` |
| `LogEntry.Format` | `LogEntry.OriginalFormat` |
| `LogEntry.Scope` | `LogEntry.Scopes` |
| `SerilogLogEntry(WriteContext)` | Created internally by `MELT.Serilog` and accessed through `ISerilogTestLoggerSink.LogEntries` |
| `SerilogLogEntry.Scope` | `SerilogLogEntry.Scopes` |
| `MELT.Xunit.LogValuesAssert` (`Contains(...)`) | `Xunit.LoggingAssert.Contains(...)` (passing `log.Properties` or `scope.Properties`) |

For a summary of the support lifecycle and breaking changes behind this release,
see the [MELT v2 release notes](RELEASE-NOTES-v2.md).
