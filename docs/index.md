# MELT documentation

<p align="center">
  <img src="assets/logo_large.png" width="256" height="256" alt="A vibrant green tree sprouting from a freshly cut stump that is resting on a melting block of ice, all set within a dark blue circle">
</p>

MELT is a testing library for applications and libraries that use
`Microsoft.Extensions.Logging`.

Start with the [quickstart](guide.md#quickstart), then use the sections on
ASP.NET Core, Serilog, or NLog when they apply to your test suite.

## Version 2

MELT v2 requires `Microsoft.Extensions.Logging` 8.0.0 or later. The core
packages target `netstandard2.0`; active support covers .NET 8, .NET 9, and
.NET 10.
The ASP.NET Core helper packages target .NET 8 and .NET 10.

For applications on Microsoft.Extensions versions earlier than 8.0, including
classic .NET Framework applications pinned to older Extensions packages, or
the legacy ASP.NET Core helper stack, use the MELT 1.x legacy release line
(latest: 1.1.0).

- [Migrate to MELT v2](MIGRATION-v2.md)
- [MELT v2 release notes](RELEASE-NOTES-v2.md)
- [Project repository](https://github.com/alefranz/MELT)
