# MELT

<!-- markdownlint-disable no-inline-html -->
<img align="right" width="256" height="256" src="logo_large.png" alt="A vibrant green tree sprouting from a freshly cut stump that is resting on a melting block of ice, all set within a dark blue circle" />
<!-- markdownlint-enable no-inline-html -->

_Testing Library for Microsoft Extensions Logging._

[![NuGet Downloads](https://img.shields.io/nuget/dt/MELT)](https://www.nuget.org/packages/MELT/)

[![MELT Nuget](https://img.shields.io/nuget/v/MELT?label=MELT&logo=nuget)](https://www.nuget.org/packages/MELT/)
[![MELT.AspNetCore Nuget](https://img.shields.io/nuget/v/MELT.AspNetCore?label=MELT.AspNetCore&logo=nuget)](https://www.nuget.org/packages/MELT.AspNetCore/)
[![MELT.Serilog Nuget](https://img.shields.io/nuget/v/MELT.Serilog?label=MELT.Serilog&logo=nuget)](https://www.nuget.org/packages/MELT.Serilog/)
[![MELT.Serilog.AspNetCore Nuget](https://img.shields.io/nuget/v/MELT.Serilog.AspNetCore?label=MELT.Serilog.AspNetCore&logo=nuget)](https://www.nuget.org/packages/MELT.Serilog.AspNetCore/)
[![MELT.Xunit Nuget](https://img.shields.io/nuget/v/MELT.Xunit?label=MELT.Xunit&logo=nuget)](https://www.nuget.org/packages/MELT.Xunit/)
[![MELT.Xunit.v3 Nuget](https://img.shields.io/nuget/v/MELT.Xunit.v3?label=MELT.Xunit.v3&logo=nuget)](https://www.nuget.org/packages/MELT.Xunit.v3/)

<!-- omit in toc -->
## About MELT

MELT is a free, open-source, testing library for the .NET Standard _Microsoft Extensions Logging_ library.
It is a solution to easily test logs.

It is a repackaging with a sweetened API and some omissions of [Microsoft.Extensions.Logging.Testing](https://github.com/aspnet/Extensions/tree/master/src/Logging/Logging.Testing), a library used internally in [ASP.NET Core](https://github.com/aspnet/AspNetCore) for testing the logging, given that [there is currently no plan to offer an official package for it](https://github.com/aspnet/Extensions/issues/672#issuecomment-532850535).

It is licensed under [Apache License 2.0](https://github.com/alefranz/MELT/blob/main/LICENSE).
Most of the code is copyrighted by the .NET Foundation as mentioned in the files headers.

If you like this project please don't forget to **star** it on [GitHub](https://github.com/alefranz/MELT) or let me know with a [tweet](https://twitter.com/AleFranz).

You can find an explanation on the advantages of using this library and the importance of testing logs on the blog post "[How to test logging when using Microsoft.Extensions.Logging](https://alessio.franceschelli.me/posts/dotnet/how-to-test-logging-when-using-microsoft-extensions-logging/)".

Please refer to the documentation for examples and compatibility details.

## Documentation

- [Documentation site](https://alefranz.github.io/MELT/)
- [MELT Quickstart](docs/guide.md#quickstart)
- [MELT Quickstart for ASP.NET Core integration tests](docs/guide.md#quickstart-for-aspnet-core-integration-tests)
- [Compatibility and sample runtime matrix](docs/guide.md#compatibility)
- [Migrate to MELT v2](docs/MIGRATION-v2.md)
- [MELT v2 release notes](docs/RELEASE-NOTES-v2.md)
- [Full MELT Documentation](docs/guide.md)

## MELT v2 support policy

MELT v2 is the current line for applications targeting .NET 10 or later and
using `Microsoft.Extensions.Logging` 10.0.0 or later (`MELT.Serilog` requires
`Serilog` 4.2.0 and `Serilog.Extensions.Logging` 10.0.0 or later). The core
packages target `netstandard2.0` and are actively supported on .NET 10 and
.NET 11. The ASP.NET Core helper packages target `net10.0` and are validated on
both .NET 10 and .NET 11.

`MELT.Xunit.v3` supports xUnit.net v3 framework versions 3.x and 4.x. MELT
uses `xunit.v3.assert` 3.0.0 as its minimum dependency and validates the same
logging assertions against both framework lines.

Use the MELT 1.x legacy release line (latest: 1.1.0) if your application must
remain on a Microsoft.Extensions dependency line earlier than 10.0 (including
.NET 8/9 or classic .NET Framework applications pinned to older Extensions
packages), Serilog 3.x, the legacy ASP.NET Core helper stack, or APIs removed
in v2. .NET Framework 4.7.2+ applications that can use Microsoft.Extensions
10.0.0 or later can instead use the v2 core packages on a best-effort basis.
See the [v2 migration guide](docs/MIGRATION-v2.md) before upgrading.

## Dependency maintenance

Dependabot opens monthly pull requests for NuGet packages and GitHub Actions.
Development-only and sample-only NuGet dependencies are grouped to keep reviews
manageable.

This library does not commit NuGet lock files. Direct package references specify
exact versions, while normal restore intentionally exercises each supported
target's transitive dependency resolution. The regular pull-request workflow,
including Dependabot pull requests, runs the `.NET 10 and .NET 11` validation
build, packs all packages, audits direct and transitive dependencies, and
exercises packed package consumers on .NET 10 and .NET 11.

Dependabot does not update consumer-facing package references. Their versions
define MELT's compatibility floors, so changes to them require a deliberate
review of the supported targets, dependency graph, and release policy.
