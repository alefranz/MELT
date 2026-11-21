# MELT v2 release notes

## Breaking support-policy change

MELT v2 targets the actively supported .NET platform:

- **Minimum actively supported runtime:** .NET 10 (LTS), with validation on
  both .NET 10 and .NET 11 (STS). Following the
  [.NET support policy](https://dotnet.microsoft.com/platform/support/policy/dotnet-core),
  .NET 8 and .NET 9 reached end of support on November 10, 2026 and are not
  actively supported by MELT v2.
- **Core packages (`MELT`, `MELT.Serilog`, `MELT.Xunit`, `MELT.Xunit.v3`):**
  Retain `netstandard2.0` and raise the minimum `Microsoft.Extensions.*`
  dependency floor (`Microsoft.Extensions.DependencyInjection`,
  `Microsoft.Extensions.Logging`, and
  `Microsoft.Extensions.Logging.Abstractions`) to `10.0.0`.
- **Serilog integration (`MELT.Serilog`):** Raises `Serilog.Extensions.Logging`
  to `10.0.0` and `Serilog` to `4.2.0`.
- **ASP.NET Core helpers (`MELT.AspNetCore`, `MELT.Serilog.AspNetCore`):**
  Target `net10.0` with `Microsoft.AspNetCore.Mvc.Testing` `10.0.0` (validated
  for both .NET 10 and .NET 11 applications) and remove the legacy ASP.NET Core
  2.1 helper stack.
- **xUnit v3/v4 support (`MELT.Xunit.v3`):** Retains `xunit.v3.assert` `3.0.1`
  as its minimum dependency and is validated with xUnit.net v3 framework
  versions 3.x and 4.x.

Consumers on earlier `Microsoft.Extensions` versions (including .NET 8/9 or
classic .NET Framework applications pinned to older Extensions packages),
Serilog 3.x, or the legacy ASP.NET Core 2.1 helper stack should remain on the
MELT 1.x legacy release line (latest: `1.1.0`). .NET Framework 4.7.2+
applications that can use `Microsoft.Extensions` 10.0.0 or later may use the v2
core `netstandard2.0` packages on a best-effort basis without CI coverage.

## Breaking API and sample removals

MELT v2 removes all APIs previously marked `[Obsolete]` in 1.x and consolidates
log capture around a single internal sink store (`ITestLoggerSink` /
`ISerilogTestLoggerSink`):

- Removed the legacy sink model (`ITestSink`, `IInternalTestSink`, `TestSink`,
  `WriteContext`, `BeginScopeContext`, and `MELTBuilder`).
- Removed `TestLogger(string, ITestSink)` (constructor is now internal; use
  `TestLoggerFactory.Create(...)` or `new TestLoggerProvider().CreateLogger(string)`).
- Removed factory, builder, provider, and ASP.NET Core compatibility APIs
  (`TestLoggerFactory(ITestSink)`, `TestLoggerFactory.LogEntries`/`Scopes`,
  `ITestLoggerFactory.LogEntries`/`Scopes`, `TestLoggerProvider._sink`,
  `TestLoggerProvider(ITestSink)`, `AddTestLogger(...)`,
  `UseTestLogging(IWebHostBuilder, ITestSink)`, `TryGetTestSink`, and
  `GetTestSink`).
- Removed singular/legacy entry aliases (`LogEntry.Format`, `LogEntry.Scope`,
  `SerilogLogEntry(WriteContext)`, and `SerilogLogEntry.Scope`).
- Removed `MELT.Xunit.LogValuesAssert` in favor of `Xunit.LoggingAssert`.
- Removed the legacy sample projects under `samples/legacy/` and the
  .NET 8/.NET 9 compatibility sample projects.

See [Migrate to MELT v2](MIGRATION-v2.md) for the two-stage upgrade guide and
complete removed-API mapping table.
