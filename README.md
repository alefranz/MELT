# MELT

<!-- markdownlint-disable no-inline-html -->
<img align="right" width="256" height="256" src="logo_large.png" alt="A stylized 3D tree with golden-yellow spherical foliage on a wooden stump against a purple background" />
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

- [MELT Quickstart](https://github.com/alefranz/MELT/blob/v1.1.0/docs/README.md#quickstart)
- [MELT Quickstart for ASP.NET Core integration tests](https://github.com/alefranz/MELT/blob/v1.1.0/docs/README.md#quickstart-for-aspnet-core-integration-tests)
- [Full MELT Documentation](https://github.com/alefranz/MELT/blob/v1.1.0/docs/README.md)

## Dependency maintenance

Dependabot opens monthly pull requests for NuGet packages and GitHub Actions.
Development-only and sample-only NuGet dependencies are grouped to keep reviews
manageable.

This library does not commit NuGet lock files. Direct package references specify
exact versions, while normal restore intentionally exercises each supported
target's transitive dependency resolution. The regular pull-request workflow,
including Dependabot pull requests, validates those updates with the same SDK
matrix as other changes.

Dependabot does not update consumer-facing package references. Their versions
define MELT's compatibility floors, so changes to them require a deliberate
review of the supported targets, dependency graph, and release policy.
