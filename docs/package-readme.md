# MELT

Testing library for `Microsoft.Extensions.Logging`.

## Install

Install version `__MELT_VERSION__` of the MELT package shown on this NuGet page.
For package selection and setup instructions, see the
[MELT documentation](https://alefranz.github.io/MELT/).

MELT v2 requires `Microsoft.Extensions.Logging` 8.0.0 or later. Create a test
logger factory, pass a logger to the system under test, then assert against the
captured entries through `loggerFactory.Sink.LogEntries`.

## Packages and compatibility

The core packages (`MELT`, `MELT.Serilog`, `MELT.Xunit`, and
`MELT.Xunit.v3`) target `netstandard2.0`. MELT v2 actively supports .NET 8,
.NET 9, and .NET 10. The ASP.NET Core helper packages target .NET 8 and .NET
10.

## Documentation

[Read the documentation for MELT __MELT_VERSION__](https://github.com/alefranz/MELT/tree/v__MELT_VERSION__/docs/guide.md).

For the latest documentation, visit [alefranz.github.io/MELT](https://alefranz.github.io/MELT/).
