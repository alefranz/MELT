# Migrate to MELT v2

MELT v2 is a breaking release focused on the currently supported .NET
platform. It is the release line for applications that use
`Microsoft.Extensions.Logging` 8.0.0 or later.

## Who should upgrade

Upgrade to v2 if your application targets .NET 8 or later and can use the
Microsoft.Extensions 8.0 dependency line. The core packages (`MELT`,
`MELT.Serilog`, `MELT.Xunit`, and `MELT.Xunit.v3`) still ship
`netstandard2.0` assets. .NET Framework 4.7.2 remains best-effort compatible
for these core packages, but is not covered by CI.

Stay on the MELT 1.x legacy release line (latest: 1.1.0) if your application
uses Microsoft.Extensions packages before 8.0, including a classic .NET
Framework application pinned to older Extensions packages, or relies on the
legacy ASP.NET Core helper dependencies. MELT v2 no longer supports the ASP.NET
Core 2.1 testing stack. A .NET Framework 4.7.2 application that can use
Microsoft.Extensions 8.0.0 or later can instead use the v2 core packages on a
best-effort basis.

## Upgrade steps

1. Update every MELT package reference to the v2 release.
2. Update direct Microsoft.Extensions package references in the test project to
   8.0.0 or later. In particular, a project that separately references
   `Microsoft.Extensions.Logging.Abstractions` should also reference a matching
   or newer `Microsoft.Extensions.Logging` package.
3. For ASP.NET Core integration tests, target .NET 8 or .NET 10 and use the
   matching modern `Microsoft.AspNetCore.Mvc.Testing` package. Do not retain
   ASP.NET Core 2.1 helper dependencies.
4. Run the test suite. Existing MELT logging assertions and test-logger setup
   APIs remain unchanged by this support-policy migration.

For a summary of the support lifecycle behind this decision, see the
[MELT v2 release notes](RELEASE-NOTES-v2.md).
