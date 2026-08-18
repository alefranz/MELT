# MELT v2 release notes

## Breaking support-policy change

MELT v2 raises the minimum Microsoft.Extensions.Logging dependency line for
the core packages to 8.0.0. The `MELT`, `MELT.Serilog`, `MELT.Xunit`, and
`MELT.Xunit.v3` packages continue to target `netstandard2.0`, and v2 actively
supports applications targeting .NET 8, .NET 9, and .NET 10. The ASP.NET Core helper
packages target .NET 8 and .NET 10 and use the matching modern ASP.NET Core
testing stacks.

This baseline follows the [.NET support policy](https://dotnet.microsoft.com/platform/support/policy/dotnet-core). .NET 8 and .NET 9 are supported through November 10, 2026, and .NET 10 LTS is supported through November 14, 2028. After November 10, 2026, active v2 support will move to .NET 10 and later in a v2 maintenance release.

The 8.0 dependency floor establishes initial package compatibility; it is not
a promise of ongoing active support for .NET 8 or .NET 9 after their support
ends. Consumers on Microsoft.Extensions versions earlier than 8.0, or on the
legacy ASP.NET Core helper stack, should use the MELT 1.x legacy release line
(latest: 1.1.0). This includes classic .NET Framework applications pinned to
older Microsoft.Extensions packages; .NET Framework 4.7.2 applications that
can use Microsoft.Extensions 8.0.0 or later may use the v2 core packages on a
best-effort basis.

`MELT.Xunit.v3` retains `xunit.v3.assert` 3.0.1 as its minimum dependency and
is validated with xUnit.net v3 framework versions 3.x and 4.x. This allows
applications to adopt xUnit 4 without forcing existing xUnit 3 users to
upgrade.

See [Migrate to MELT v2](MIGRATION-v2.md) for upgrade guidance.
