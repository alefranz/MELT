# MELT v2 cleanup

This document tracks compatibility code and obsolete APIs to remove before the
MELT v2 release. Because these changes break source and binary compatibility,
v2 is the right boundary for removing them rather than carrying them into
another major release.

For the current branch assessment, validation evidence, and prioritized release
checklist, see [Release readiness assessment (2026-10-19)](#release-readiness-assessment-2026-10-19)
below. The original cleanup checklist remains the detailed implementation plan.

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

- [ ] Remove `ITestSink`.
- [ ] Remove `IInternalTestSink`.
- [ ] Remove `TestSink`.
- [ ] Remove `WriteContext`, including its obsolete singular `Scope` property.
- [ ] Remove `BeginScopeContext`.
- [ ] Remove `MELTBuilder.CreateTestSink()` and
      `MELTBuilder.CreateTestSink(Action<TestLoggerOptions>)`.

These types expose implementation details and force the current code to use
obsolete types internally. Replace them with a non-public capture model owned
by the provider. The public read-only surface should remain
`ITestLoggerSink`, exposing `LogEntries`, `Scopes`, and `Clear()`.

**Refactoring potential:** this removes the split between the public sink,
internal sink, and sink accessor. A single internal capture store can accept
logs and scopes while `ITestLoggerSink` presents its read-only view. That
should eliminate most `CS0612` suppressions and make ownership and dependency
injection registrations easier to understand.

### Logger factory compatibility APIs

- [ ] Remove `MELTBuilder.CreateLoggerFactory()` and
      `MELTBuilder.CreateLoggerFactory(Action<TestLoggerOptions>)`.
- [ ] Remove `TestLoggerFactory(ITestSink)`.
- [ ] Remove `TestLoggerFactory.LogEntries`.
- [ ] Remove `TestLoggerFactory.Scopes`.
- [ ] Remove `ITestLoggerFactory.LogEntries`.
- [ ] Remove `ITestLoggerFactory.Scopes`.

Consumers should create factories through `TestLoggerFactory.Create(...)` and
read captured data through `ITestLoggerFactory.Sink`.

**Refactoring potential:** removing the compatibility constructor allows all
factory instances to use one construction path and one dependency-injection
layout. The duplicated forwarding properties disappear, leaving the factory
responsible only for logger creation, provider registration, disposal, and
access to its sink.

### Logging builder compatibility APIs

- [ ] Remove `AddTestLogger()`.
- [ ] Remove `AddTestLogger(Action<TestLoggerOptions>)`.
- [ ] Remove `AddTestLogger(ITestSink)`.
- [ ] Remove commented-out `AddTestLogger` overload implementations.
- [ ] Rename private helpers if their names still refer to the removed
      `TestLogger` compatibility API.

Consumers should use `AddTest()` or `AddTest(Action<TestLoggerOptions>)`.

**Refactoring potential:** `AddTest` can register the internal capture store,
provider, and `ITestLoggerSink` directly. It will no longer need to construct
obsolete sink types or suppress warnings around provider creation.

### Logger provider compatibility APIs

- [ ] Remove the public `TestLoggerProvider._sink` field.
- [ ] Remove `TestLoggerProvider(ITestSink)`.
- [ ] Replace the field with a private, modern capture-store dependency.
- [ ] Reassess whether `TestLoggerProvider` needs to remain publicly
      constructible once the compatibility constructor is gone.
- [ ] Remove the stale `TODO` associated with the provider constructor.

The supported public accessor is `ITestLoggerProvider.Sink`.

**Refactoring potential:** the provider can have one initialization strategy
instead of maintaining a public mutable implementation contract. If direct
construction is not a required scenario, provider creation can be centralized
under the logging-builder extensions.

### ASP.NET Core compatibility APIs

- [ ] Remove `TryGetTestSink`.
- [ ] Remove `GetTestSink`.
- [ ] Remove `UseTestLogging(IWebHostBuilder, ITestSink)`.

Consumers should use `TryGetTestLoggerSink`, `GetTestLoggerSink`, and the
`UseTestLogging` overloads that create and register their own sink.

**Refactoring potential:** ASP.NET Core integration no longer needs to pass an
obsolete concrete sink through web-host configuration. Sink discovery can use
only the registered `ITestLoggerSink`, reducing overloads and service lookup
branches.

### Captured entry aliases

- [ ] Remove `LogEntry.Format`; use `LogEntry.OriginalFormat`.
- [ ] Remove `LogEntry.Scope`; use `LogEntry.Scopes`.
- [ ] Remove `SerilogLogEntry.Scope`; use `SerilogLogEntry.Scopes`.
- [ ] Remove the public `SerilogLogEntry(WriteContext)` constructor together
      with `WriteContext`.

**Refactoring potential:** removing singular-scope compatibility means the
capture pipeline only needs to model the complete scope collection. A modern
internal entry representation can be shared by MELT and MELT.Serilog without
exposing the old write context.

### xUnit compatibility APIs

- [ ] Remove `LogValuesAssert` and all of its `Contains` overloads.
- [ ] Remove the corresponding `LogValuesAssertTest` coverage.

Consumers should use `Xunit.LoggingAssert`.

**Refactoring potential:** MELT.Xunit will have one assertion API and one test
suite, avoiding forwarding methods and duplicate documentation.

## Internal cleanup enabled by the removals

- [ ] Replace obsolete implementation types in `TestLogger`,
      `TestLoggerSinkAccessor`, `TestSinkOptions`, `BeginScope`, `LogEntry`, and
      the Serilog integration.
- [ ] Remove all `CS0612` and `CS0618` pragma suppressions from `src`.
- [ ] Remove obsolete-specific test helpers and warning suppressions from
      samples and tests.
- [ ] Delete commented-out code in `MELTBuilder` and
      `MELTLoggingBuilderExtensions`.
- [ ] Remove `MELTBuilder` entirely if `CreateOptions` and sink construction
      move to focused internal helpers.
- [ ] Prefer a single immutable captured-entry model where practical, while
      preserving thread-safe capture and `Clear()` behavior.
- [ ] Ensure public APIs have XML documentation without broad `CS1591`
      suppressions where touched by this work.

## Legacy sample removal

- [ ] Remove `samples/legacy/SampleLibrary.LegacyTests`.
- [ ] Remove `samples/legacy/SampleLibrary.HandRolledTests`.
- [ ] Remove the `legacy` solution folder and both project entries from
      `MELT.sln`.
- [ ] Remove documentation, build configuration, and CI references to these
      projects.
- [ ] Verify no remaining current sample uses an obsolete MELT API.

The MELT 1.x branch and its documentation remain the reference for legacy
usage. The v2 repository and solution should demonstrate only supported v2
patterns.

## Compatibility floors to review separately

The following are old by design and should not be changed merely because they
have newer releases:

- `MELT.Xunit` references `xunit.assert` 2.2.0 and `xunit.abstractions` 2.0.2
  to preserve xUnit v2 compatibility.
- `MELT.Serilog` references Serilog 3.1.1 as its consumer compatibility floor.
- Core packages target `netstandard2.0` for the documented best-effort .NET
  Framework compatibility scenario.

Any change to these floors needs an explicit support-policy decision,
consumer-package tests, and migration documentation. They are not part of the
obsolete API cleanup by default.

## Documentation

- [ ] Document the recommended two-stage consumer migration:
  1. Upgrade all MELT packages to the latest 1.x release.
  2. Resolve every MELT obsolete warning by adopting the replacement APIs,
     which are already available in 1.x.
  3. Verify the test suite still passes on 1.x.
  4. Upgrade all MELT packages together to v2 and apply the new framework and
     dependency requirements.
- [ ] Explain that this sequence separates API migration from framework and
      package compatibility changes, making failures easier to diagnose.
- [ ] Encourage consumers to treat MELT obsolete warnings as errors temporarily
      so no removed API remains before the v2 upgrade.
- [ ] Add a complete removed-API table to `docs/MIGRATION-v2.md`, pairing each
      removed API with its supported replacement.
- [ ] Update `docs/RELEASE-NOTES-v2.md` with the breaking API and sample
      removals.
- [ ] Remove or revise the migration-guide statement that existing logging
      assertion and setup APIs remain unchanged.
- [ ] Check `README.md`, `docs/guide.md`, `docs/index.md`, and
      `docs/package-readme.md` for obsolete names and legacy sample links.
- [ ] Retain clear guidance that consumers requiring the removed APIs should
      remain on MELT 1.x.

## Suggested implementation order

1. Add the staged 1.x-to-v2 migration instructions, removed-API table, and
   tests for the public replacement APIs.
2. Introduce the internal capture model and migrate MELT and MELT.Serilog to
   it.
3. Remove legacy sink and provider APIs.
4. Remove factory, builder, and ASP.NET Core compatibility APIs.
5. Remove captured-entry aliases and `LogValuesAssert`.
6. Remove obsolete tests, warning suppressions, and dead code.
7. Remove legacy samples and their solution/build references.
8. Pack every package and run package-consumer tests against the resulting
   artifacts.

## Completion criteria

- [ ] `git grep '\[Obsolete' -- 'src/**/*.cs'` returns no matches.
- [ ] `git grep 'CS0612\|CS0618' -- 'src/**/*.cs'` returns no matches.
- [ ] `samples/legacy` no longer exists and the solution contains no legacy
      sample projects.
- [ ] The solution builds with warnings treated as errors.
- [ ] Unit, integration, xUnit v2, xUnit v3, xUnit v4, Serilog, and NLog
      coverage passes.
- [ ] Packed-package consumer tests pass for every supported target framework.
- [ ] Public API and package contents contain none of the removed types or
      members.
- [ ] Migration and release documentation lists every breaking removal and
      its replacement.

## Release readiness assessment (2026-10-19)

**Verdict: modernization is substantially implemented, but the branch is not
ready to release.** The main remaining implementation is the obsolete-API and
capture-pipeline cleanup above. There are also current build/security failures,
gaps in release validation, and migration documentation to finish.

This assessment covers `feature/v2-modernization` at
`8721336a9c148700ba4327a247ad6bc3d7dff0f3`, before this documentation update.
The working tree was clean. The branch contains 27 commits beyond `main` and
matches the head of the open draft [MELT v2 PR #50](https://github.com/alefranz/MELT/pull/50).
Its last implementation/planning work was on August 18, 2026; the final commit
added this cleanup plan, rather than carrying it out.

### Recovered release intent

`CLEANUP.md` is the second phase of the work. The broader modernization plan
was in `BACKLOG.md`, deleted in `54b74d5` after its implementation items were
marked complete. To recover the final version, run:

```powershell
git show 54b74d5^:BACKLOG.md
```

The earliest plan proposed a .NET 10 / Microsoft.Extensions 10 baseline.
That was superseded by the .NET 8 dependency floor and .NET 8/9/10 runtime
policy now in the code, migration guide, release notes, and PR description.
Do not restart the original .NET 10-only dependency migration by accident.

The intended release is a bounded transition: retire the old compatibility
surface and simplify the internals so future development has less baggage.
The modernization work provides the platform for that cleanup; shipping the
branch unchanged would leave that central goal unfinished.

### What is already implemented

| Area | Evidence at the assessed revision |
| --- | --- |
| Core compatibility baseline | `MELT`, `MELT.Serilog`, `MELT.Xunit`, and `MELT.Xunit.v3` retain `netstandard2.0`; Microsoft.Extensions references use the 8.0 floor. |
| Modern ASP.NET Core helpers | Both helper packages ship `net8.0` and `net10.0` assets with matching MVC testing dependency groups and framework references; the old reflective service lookup is gone. |
| Retired full-framework web sample | The ASP.NET Core 2.1 / `net481` sample and `MELT.CI.slnf` are removed. This is separate from `samples/legacy`, which still exists. |
| Runtime and provider coverage | The solution includes .NET 8/9/10 samples, NLog, Serilog 3.1.1 and 4.4.0 scenarios, and xUnit v2/v3/v4 samples. |
| Build and package maintenance | Development dependencies were refreshed; monthly Dependabot rules deliberately preserve published dependency floors; CI installs all three runtime lines. |
| Release machinery | Packing, direct/transitive vulnerability scanning, packed-consumer checks, and manual local-feed publishing rehearsal are implemented. Their present limitations are listed below. |
| Documentation infrastructure | Migration/release documents, version-substituted NuGet READMEs, and a strict MkDocs site build exist. The migration content still predates the API cleanup. |

### Validation performed for this assessment

Local validation used Windows and SDK **10.0.402**, with .NET 8, 9, and 10
runtimes installed. This is runtime/target coverage built with the .NET 10
SDK, not proof that the whole solution builds using the .NET 8 SDK.

| Check | Result and limits |
| --- | --- |
| `dotnet format --verify-no-changes --no-restore MELT.sln` | Exit 0; reported workspace-loading warnings. |
| `dotnet build --configuration Release MELT.sln` | **Failed** with `NU1902` for `Microsoft.Build.Tasks.Git` 10.0.301 in the ASP.NET Core helper projects. |
| Diagnostic Release build | Passed after allowing `NU1902` as a warning while preserving the existing `CS0612`/`CS0618` warning exceptions. This is not a passing release gate. |
| `dotnet test --no-build --no-restore --configuration Release MELT.sln` | **238 passed, 0 failed, 0 skipped**, across 18 project/framework runs, using the diagnostic build. Includes the legacy samples pending removal. |
| Diagnostic pack | All six `.nupkg` and six `.snupkg` files produced with version `2.0.0-assessment.20261019`; used the same temporary warning exception and `GITHUB_ACTIONS=true`. |
| Existing packed-consumer script | Passed for .NET 8, 9, and 10 using those local packages. The .NET 9 consumer currently checks only the four core packages. |
| Package inspection | Expected target assets, README, icon, and dependency groups are present. SourceLink is not a published runtime dependency. Obsolete APIs still exist in the source being packed. |
| Existing vulnerability gate | **Failed**: reports `Microsoft.Build.Tasks.Git` 10.0.301 across all six production projects. No other vulnerable package was reported in this scan. |
| `mkdocs build --strict` | Passed using an isolated installation of `mkdocs-material`, matching the docs workflow's unpinned installation approach. Does not validate external links or migration accuracy. |

The diagnostic build/pack command-line exception was:

```powershell
'-p:WarningsNotAsErrors=NU1902%3BCS0612%3BCS0618'
```

No project, dependency, or CI settings were changed to obtain these results.
Local evidence is in ignored `artifacts-assessment-*.log` files and
`artifacts/assessment/` (test results, packages, vulnerability JSON, and docs
output). These files are assessment artifacts, not release candidates.

The assessed head's [GitHub Actions run](https://github.com/alefranz/MELT/actions/runs/32190919924)
reports a successful validation job. Its **pack, consumer-smoke, and publish
jobs were skipped**. That historical green result does not establish current
release readiness.

### Remaining work, in priority order

#### 1. Restore trustworthy build and release gates

- [ ] Update the SourceLink build-tool dependency in `src/Directory.Build.props`
      so it no longer resolves `Microsoft.Build.Tasks.Git` 10.0.301. The
      [Microsoft advisory](https://github.com/advisories/GHSA-23fw-v26w-5fgq)
      lists 10.0.303 as the fix on that version line. Re-run the normal build
      and the transitive scan; do not retain the diagnostic warning exception.
      This is a build-tool update, not a reason to raise consumer package floors.
- [ ] Make `build.ps1` stop and return a failure when **any** native command
      fails. It currently runs format, build, and test without checking each
      exit code. A later successful test command can mask an earlier failure,
      including by testing old build output. Verify failure propagation.
- [ ] Ensure the final candidate actually runs packing, the vulnerability gate,
      and packed consumers. They currently run only on pushes to `main`/tags or
      manual dispatch, not PR validation. Prefer enabling these checks on PRs
      for ongoing maintenance; at minimum record a successful manual run for
      the final candidate before tagging.

#### 2. Carry out the compatibility removal and internal simplification

- [ ] Complete the original obsolete API, internal cleanup, and legacy sample
      checklists above. All **34 `[Obsolete]` attributes in `src`** were still
      present at assessment time, together with the warning suppressions and
      the old sink/accessor construction paths. None of those removal groups
      can be considered complete yet.
- [ ] Include **`TestLogger(string, ITestSink)`** in the public API review.
      The constructor is public and not itself marked obsolete, but its sink
      type is scheduled for removal. Decide and document its removal or
      replacement; an obsolete-attribute search alone misses this break.
- [ ] Decide the supported direct-construction surface for `TestLogger` and
      `TestLoggerProvider` before replacing the internals. In particular,
      `TestLoggerProvider()` is a current non-obsolete public constructor.
      Keep any intentional removal visible in the migration guide.
- [ ] Migrate both MELT and MELT.Serilog to the new capture store together.
      Preserve filtering, nested/async scopes, captured properties and format,
      sink discovery, disposal, and `Clear()` behavior through the supported
      factory/builder APIs. Existing tests provide a starting point; add
      focused regression coverage where the new ownership model needs it.
- [ ] Remove obsolete-warning exemptions in `src/Directory.Build.props`,
      `test/Directory.Build.props`, and `samples/Directory.Build.props` when
      their callers are migrated, as well as the source-level pragmas. The
      current warnings-as-errors setting has these deliberate exceptions.
- [ ] Review the final public API and packed assemblies for removed symbols
      and accidental exposure of new internal capture types. Update the
      removed-API table for breaks beyond explicitly obsolete members.

#### 3. Finish consumer migration guidance

- [ ] Implement the Documentation checklist above: latest 1.x first, resolve
      obsolete usage and run tests there, then upgrade all MELT packages to v2.
- [ ] Replace the migration guide's current claim that logging assertion and
      logger setup APIs remain unchanged. It describes only the earlier
      support-policy work and will be false after the cleanup.
- [ ] Add every removed API and its replacement to the migration guide and
      release notes, including the `TestLogger` constructor decision. Check
      the README, guide, package README, and samples against the final API.

#### 4. Close the gap between package smoke tests and release confidence

- [ ] Exercise actual packaged ASP.NET Core logging capture, not just builder
      registration. `test/PackageConsumers/Program.cs` currently calls
      `WebApplication.CreateBuilder().WebHost.UseTestLogging().UseSerilogTestLogging()`
      but never starts a host, sends a request, or retrieves its sink.
      Re-run standard, NLog, and Serilog integration scenarios against packed
      dependencies on the retained helper targets, as the recovered backlog
      required. Existing full integration tests use project references.
- [ ] Verify the packaged Serilog sink captures entries independently. The
      current smoke test asserts `factory.Sink.LogEntries` after `AddSerilogTest`,
      which checks the core sink; it never asserts `ISerilogTestLoggerSink`.
- [ ] After cleanup, re-run formatting, the ordinary warnings-as-errors
      Release build, all retained tests, six-package packing, transitive audit,
      packed consumers, package/API inspection, and the strict docs build.
      Record the tested commit and results without warning overrides.

#### 5. Settle release timing and rehearse publication

- [ ] Reconfirm active runtime support for the actual release date. The current
      policy includes .NET 8/9/10, but [.NET 8 and .NET 9 support ends on
      November 10, 2026](https://dotnet.microsoft.com/en-us/platform/support/policy/dotnet-core).
      The deleted backlog explicitly planned a .NET 10-and-later active-support
      transition after that date. If v2 ships later, resolve that policy and
      update CI/docs before release; do not mechanically equate dropping active
      runtime support with raising the 8.0 package dependency floor.
- [ ] Run the existing manual `dry_run_publish` workflow on the final candidate
      and retain evidence of pack, audit, consumers, and local-feed push. It
      does not prove NuGet credentials, so confirm the real publishing setup
      is available before the release tag is pushed.
- [ ] Verify the chosen v2 tag produces the intended package version and valid
      versioned README links, and that migration/release documentation is
      available at the URLs embedded in the packages. The release-notes URL
      currently points at `main`, so account for merge/publication order.
- [ ] Merge/review the final candidate and publish all six packages from the
      tested release revision. A tag push currently triggers real publishing;
      it should follow the checks above, not serve as the first packaging test.

### Scope boundary

Do not make this release depend on new logging features, a broad redesign of
assertions, abandoning `netstandard2.0`, raising the xUnit/Serilog compatibility
floors merely to use newer versions, full-framework CI, or adopting lock files.
Those are separate decisions. The remaining v2 work is the bounded cleanup,
accurate migration guidance, and reliable validation/publication described
above. Once those gates pass, this transition release has met its purpose.
