# MELT v2 cleanup

This document tracks compatibility code and obsolete APIs to remove before the
MELT v2 release. Because these changes break source and binary compatibility,
v2 is the right boundary for removing them rather than carrying them into
another major release.

For the updated branch status, validation evidence, and release-readiness
summary, see [Release readiness assessment (2026-11-21)](#release-readiness-assessment-2026-11-21)
below.

## Goals

- Remove every API currently marked with `ObsoleteAttribute`.
- Remove implementation code that exists only to support those APIs.
- Remove legacy samples that demonstrate APIs and dependency lines retained by
  MELT 1.x.
- Simplify the remaining implementation around `ITestLoggerSink`,
  `TestLoggerFactory.Create`, `AddTest`, and `UseTestLogging`.
- Record all breaking changes in the v2 migration guide and release notes.
- Finish with no obsolete-warning suppressions in production code.

## Obsolete API removal

### Legacy sink model

- [x] Remove `ITestSink`.
- [x] Remove `IInternalTestSink`.
- [x] Remove `TestSink`.
- [x] Remove `WriteContext`, including its obsolete singular `Scope` property.
- [x] Remove `BeginScopeContext`.
- [x] Remove `MELTBuilder.CreateTestSink()` and
      `MELTBuilder.CreateTestSink(Action<TestLoggerOptions>)`.

These types exposed implementation details and forced the code to use obsolete
types internally. They are replaced by a single internal `TestLoggerSink`
capture store owned by `TestLoggerProvider` / DI registrations, with
`ITestLoggerSink` (`LogEntries`, `Scopes`, and `Clear()`) as the public
read-only view.

### Logger factory compatibility APIs

- [x] Remove `MELTBuilder.CreateLoggerFactory()` and
      `MELTBuilder.CreateLoggerFactory(Action<TestLoggerOptions>)`.
- [x] Remove `TestLoggerFactory(ITestSink)`.
- [x] Remove `TestLoggerFactory.LogEntries`.
- [x] Remove `TestLoggerFactory.Scopes`.
- [x] Remove `ITestLoggerFactory.LogEntries`.
- [x] Remove `ITestLoggerFactory.Scopes`.

Consumers create factories through `TestLoggerFactory.Create(...)` and read
captured data through `ITestLoggerFactory.Sink`.

### Logging builder compatibility APIs

- [x] Remove `AddTestLogger()`.
- [x] Remove `AddTestLogger(Action<TestLoggerOptions>)`.
- [x] Remove `AddTestLogger(ITestSink)`.
- [x] Remove commented-out `AddTestLogger` overload implementations.
- [x] Rename private helpers if their names still refer to the removed
      `TestLogger` compatibility API.

Consumers use `AddTest()` or `AddTest(Action<TestLoggerOptions>)` (and
`AddSerilogTest()` / `AddSerilogTest(Action<TestLoggerOptions>)` for Serilog).
`AddTestLoggerProvider` registers `TestLoggerSink`, `ITestLoggerSink`, and
`TestLoggerProvider` directly and replaces any prior `TestLoggerProvider` /
`TestLoggerSink` / `ITestLoggerSink` registration.

### Logger provider compatibility APIs

- [x] Remove the public `TestLoggerProvider._sink` field.
- [x] Remove `TestLoggerProvider(ITestSink)`.
- [x] Replace the field with a private, modern capture-store dependency.
- [x] Reassess whether `TestLoggerProvider` needs to remain publicly
      constructible once the compatibility constructor is gone.
- [x] Remove the stale `TODO` associated with the provider constructor.

Decision: `TestLoggerProvider()` remains public (initializing a default internal
`TestLoggerSink`, matching the 1.x deprecation guidance on
`TestLoggerProvider(ITestSink)`), while `TestLoggerProvider(TestLoggerSink)` and
`TestLogger(string, TestLoggerSink)` are internal. `TestLogger(string, ITestSink)`
has been removed and documented in the migration guide. The supported public
accessor on `TestLoggerProvider` is `ITestLoggerProvider.Sink`.

### ASP.NET Core compatibility APIs

- [x] Remove `TryGetTestSink`.
- [x] Remove `GetTestSink`.
- [x] Remove `UseTestLogging(IWebHostBuilder, ITestSink)`.

Consumers use `TryGetTestLoggerSink`, `GetTestLoggerSink`, and the
`UseTestLogging` overloads that create and register their own sink.

### Captured entry aliases

- [x] Remove `LogEntry.Format`; use `LogEntry.OriginalFormat`.
- [x] Remove `LogEntry.Scope`; use `LogEntry.Scopes`.
- [x] Remove `SerilogLogEntry.Scope`; use `SerilogLogEntry.Scopes`.
- [x] Remove the public `SerilogLogEntry(WriteContext)` constructor together
      with `WriteContext`.

### xUnit compatibility APIs

- [x] Remove `LogValuesAssert` and all of its `Contains` overloads.
- [x] Remove the corresponding `LogValuesAssertTest` coverage.

Consumers use `Xunit.LoggingAssert`.

## Internal cleanup enabled by the removals

- [x] Replace obsolete implementation types in `TestLogger`,
      `TestLoggerSinkAccessor`, `TestSinkOptions`, `BeginScope`, `LogEntry`, and
      the Serilog integration.
- [x] Remove all `CS0612` and `CS0618` pragma suppressions from `src`.
- [x] Remove obsolete-specific test helpers and warning suppressions from
      samples and tests.
- [x] Delete commented-out code in `MELTBuilder` and
      `MELTLoggingBuilderExtensions`.
- [x] Remove `MELTBuilder` entirely after moving `CreateOptions` and sink
      construction to focused internal helpers (`TestLoggerOptions.Create` and
      `TestLoggerSink`).
- [x] Prefer a single captured-entry model while preserving thread-safe capture
      and `Clear()` behavior.
- [x] Ensure public APIs have XML documentation without broad `CS1591`
      suppressions where touched by this work.

## Legacy sample removal

- [x] Remove `samples/legacy/SampleLibrary.LegacyTests`.
- [x] Remove `samples/legacy/SampleLibrary.HandRolledTests`.
- [x] Remove the `legacy` solution folder and both project entries from
      `MELT.sln`.
- [x] Remove documentation, build configuration, and CI references to these
      projects.
- [x] Verify no remaining current sample uses an obsolete MELT API.

The MELT 1.x branch and its documentation remain the reference for legacy
usage. The v2 repository and solution demonstrate only supported v2 patterns.

## Compatibility floors and target baseline

The v2 compatibility baseline (following the November 10, 2026 end of support
for .NET 8 and .NET 9) is:

- **Active runtime support:** .NET 10 (LTS) minimum, actively validated on
  .NET 10 and .NET 11 (STS). .NET 8/9-specific sample projects and consumer
  smoke projects have been removed.
- **Core packages (`MELT`, `MELT.Serilog`, `MELT.Xunit`, `MELT.Xunit.v3`):**
  Retain `netstandard2.0` for broad compatibility (including best-effort
  .NET Framework 4.7.2+ usage when paired with Microsoft.Extensions 10.0.0+,
  without CI coverage).
- **Microsoft.Extensions dependency floor:** `10.0.0` across
  `Microsoft.Extensions.DependencyInjection`, `Microsoft.Extensions.Logging`,
  and `Microsoft.Extensions.Logging.Abstractions`.
- **Serilog compatibility floor (`MELT.Serilog`):** Raised to `Serilog` `4.2.0`
  and `Serilog.Extensions.Logging` `10.0.0` (aligned with the Extensions 10
  baseline).
- **ASP.NET Core helpers (`MELT.AspNetCore`, `MELT.Serilog.AspNetCore`):**
  Target `net10.0` with `Microsoft.AspNetCore.Mvc.Testing` `10.0.0`, validated
  from both `.NET 10` and `.NET 11` applications.
- **xUnit floors preserved:** `MELT.Xunit` retains `xunit.assert` `2.2.0` and
  `xunit.abstractions` `2.0.2`; `MELT.Xunit.v3` retains `xunit.v3.assert`
  `3.0.1` and is validated with xUnit.net v3 framework 3.x and 4.x.

## Documentation

- [x] Document the recommended two-stage consumer migration:
  1. Upgrade all MELT packages to the latest 1.x release.
  2. Resolve every MELT obsolete warning by adopting the replacement APIs,
     which are already available in 1.x.
  3. Verify the test suite still passes on 1.x.
  4. Upgrade all MELT packages together to v2 and apply the new framework and
     dependency requirements.
- [x] Explain that this sequence separates API migration from framework and
      package compatibility changes, making failures easier to diagnose.
- [x] Encourage consumers to treat MELT obsolete warnings as errors temporarily
      so no removed API remains before the v2 upgrade.
- [x] Add a complete removed-API table to `docs/MIGRATION-v2.md`, pairing each
      removed API with its supported replacement (including
      `TestLogger(string, ITestSink)`).
- [x] Update `docs/RELEASE-NOTES-v2.md` with the breaking API, compatibility
      baseline, and sample removals.
- [x] Remove the migration-guide statement that existing logging assertion and
      setup APIs remain unchanged.
- [x] Check `README.md`, `docs/guide.md`, `docs/index.md`, and
      `docs/package-readme.md` for obsolete names, legacy sample links, and
      outdated .NET 8/9 or Extensions 8.0 policy wording.
- [x] Retain clear guidance that consumers requiring earlier dependencies or
      the removed APIs should remain on MELT 1.x.

## Completion criteria

- [x] `git grep '\[Obsolete' -- 'src/**/*.cs'` returns no matches.
- [x] `git grep 'CS0612\|CS0618' -- 'src/' 'test/' 'samples/'` returns no matches.
- [x] `samples/legacy` no longer exists and the solution contains no legacy
      sample projects.
- [x] The solution builds with warnings treated as errors.
- [x] Unit, integration, xUnit v2, xUnit v3, xUnit v4, Serilog, and NLog
      coverage passes.
- [x] Packed-package consumer tests pass for every supported target runtime
      (`.NET 10` and `.NET 11`).
- [x] Public API and package contents contain none of the removed types or
      members.
- [x] Migration and release documentation lists every breaking removal and
      its replacement.

## Release readiness assessment (2026-11-21)

**Verdict: v2 modernization, obsolete-API cleanup, .NET 10/11 baseline updates,
and release-gate repairs are implemented and verified locally.**

### Implemented updates since the 2026-10-19 assessment

#### 1. Trustworthy build and release gates

- [x] Updated `Microsoft.SourceLink.GitHub` in `src/Directory.Build.props` from
      `10.0.301` to `10.0.303`, resolving the `NU1902` vulnerability on
      `Microsoft.Build.Tasks.Git`.
- [x] Updated `build.ps1` and `pack.ps1` to set `$ErrorActionPreference = 'Stop'`
      and propagate `$LASTEXITCODE` on every native `dotnet` command.
- [x] Updated `.github/workflows/build.yml` and
      `.github/actions/setup-melt-dotnet/action.yml` to run the `.NET 10 and
      .NET 11` matrix and execute `pack` (including transitive vulnerability
      scanning) and `consumer-smoke` on pull requests as well as pushes and
      manual dispatches. Publishing remains gated on tags and `workflow_dispatch`.

#### 2. .NET 10/11 baseline and dependency floors

- [x] Retired `.NET 8` and `.NET 9` targets, samples (`SampleLibrary.Net8.Tests`,
      `SampleWebApplication.Net8*`, `SampleWebApplication.Net9*`), and consumer
      projects (`Consumer.Net8`, `Consumer.Net9`).
- [x] Updated `MELT.AspNetCore` and `MELT.Serilog.AspNetCore` to target
      `net10.0` with `Microsoft.AspNetCore.Mvc.Testing` `10.0.0`.
- [x] Raised `Microsoft.Extensions.*` floors to `10.0.0` in `MELT` and
      `MELT.Serilog`, and raised `Serilog` (`4.2.0`) and
      `Serilog.Extensions.Logging` (`10.0.0`) in `MELT.Serilog`.
- [x] Added `test/PackageConsumers/Consumer.Net11` (`net11.0`) and updated
      `.github/scripts/test-packed-consumers.ps1` to validate both `.NET 10`
      and `.NET 11` against all six packed packages.

#### 3. Obsolete API removal and internal simplification

- [x] Removed all 34 `[Obsolete]` attributes across `src/`, all `CS0612` /
      `CS0618` warning exemptions and pragmas across `src/`, `test/`, and
      `samples/`, and all legacy/obsolete types (`ITestSink`,
      `IInternalTestSink`, `TestSink`, `WriteContext`, `BeginScopeContext`,
      `MELTBuilder`, `TestLoggerSinkAccessor`, `LogValuesAssert`).
- [x] Introduced `internal sealed class TestLoggerSink : ITestLoggerSink` as
      the single capture store shared by `MELT` and `MELT.Serilog`, updated
      `TestLoggerOptions` and `SerilogLogEntry` directly around `LogEntry` and
      `BeginScope`, kept `TestLoggerProvider()` public (with internal
      `TestLoggerProvider(TestLoggerSink)`), and made `TestLogger(string,
      TestLoggerSink)` internal.
- [x] Expanded `test/MELT.Tests/TestSinkOptionsTest.cs` with coverage for
      chained filtering, `Clear()`, direct `TestLoggerProvider()`,
      `GetTestLoggerSink()`, and repeated `AddTest(...)` registration.

#### 4. End-to-end packed consumer and documentation validation

- [x] Expanded `test/PackageConsumers/Program.cs` to validate core logging and
      `Clear()`, direct `ISerilogTestLoggerSink` capture, `xunit2` and `xunit3`
      `LoggingAssert`, and live HTTP request log/scope capture through
      `WebApplicationFactory` for both `MELT.AspNetCore` (`UseTestLogging` +
      `GetTestLoggerSink`) and `MELT.Serilog.AspNetCore` (`UseSerilogTestLogging`
      + `GetSerilogTestLoggerSink`) on both `.NET 10` and `.NET 11`.
- [x] Updated `docs/MIGRATION-v2.md`, `docs/RELEASE-NOTES-v2.md`, `README.md`,
      `docs/guide.md`, `docs/index.md`, and `docs/package-readme.md` for the
      final v2 API surface and `.NET 10` / `.NET 11` policy.

#### 5. Remaining pre-tag release actions

- [ ] Run the full GitHub Actions workflow (including `validate`, `pack`,
      `consumer-smoke`, and manual `dry_run_publish`) on the merged/final PR
      commit, and verify NuGet.org publishing credentials before pushing the v2
      release tag.
- [ ] Merge to `main` so package release-notes and documentation links resolve
      on `main` before tagging and publishing all six packages to NuGet.org.
