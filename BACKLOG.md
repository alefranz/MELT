# MELT v2 Backlog

## Release intent

Prepare a breaking MELT v2 release for November 2026, after .NET 8 and .NET 9
reach end of support. v2 actively supports .NET 10 and later, and requires
Microsoft.Extensions.Logging 10.0 or later. The core packages retain broad
runtime compatibility through `netstandard2.0`; .NET Framework 4.7.2 is a
best-effort core-package scenario, not an actively tested support target.

This branch is the integration branch for the release. Keep each backlog item
in a focused pull request so package, build, and compatibility changes can be
reviewed independently.

## Compatibility decisions

- Keep `MELT`, `MELT.Serilog`, `MELT.Xunit`, and `MELT.Xunit.v3` on
  `netstandard2.0`.
- Set the Microsoft.Extensions.Logging, Logging.Abstractions, and dependency
  injection package references used by the core packages to a minimum version
  of `10.0.0`. Do not use version ranges.
- Target `net10.0` for tests, retained samples, and the ASP.NET Core helper
  packages.
- Treat .NET Framework 4.7.2 as best-effort compatibility for the
  `netstandard2.0` core packages only. Do not add a `net472` target or a
  dedicated Windows CI job.
- Remove all `net481` projects and the ASP.NET Core 2.1 sample instead of
  preserving an untested historical compatibility promise.
- Retire direct package references to ASP.NET Core 2.1. The ASP.NET Core
  helper packages must use the .NET 10 ASP.NET Core testing stack.
- Document 1.x as the legacy line for consumers using Microsoft.Extensions
  versions earlier than 10.0 or legacy ASP.NET Core helpers. Do not backport the
  v2 target or dependency changes to 1.x.

## Release backlog

### 1. Retire legacy ASP.NET Core 2.1 assets and simplify CI

**Scope**

- Remove `samples/2.1/SampleWebApplication2_1` and its `net481` integration
  test project from the repository, solutions, filters, scripts, CI, and
  documentation.
- Remove obsolete ASP.NET Core 2.1 conditional build logic, package references,
  and compatibility assertions discovered during the v2 target migration.
- Determine whether `MELT.CI.slnf` is still needed once the full-framework
  projects are gone. Remove it if it no longer has a distinct CI purpose.
- Do not add Windows `net481` CI coverage. The core package's .NET Framework
  compatibility is best effort, and the retired ASP.NET Core 2.1 sample must
  not define the v2 test matrix.

**Done when**

- `rg 'net481|AspNetCore.*2\\.1|ASP.NET Core 2.1'` finds no active project
  configuration or obsolete support claim outside the migration notes.
- The solution and CI no longer need a special full-framework exclusion.

### 2. Establish the core package compatibility baseline

**Scope**

- Retain `netstandard2.0` for the core MELT, Serilog, and xUnit helper packages.
- Upgrade the core Microsoft.Extensions.Logging, Logging.Abstractions, and
  dependency-injection references to a minimum version of `10.0.0`, without
  adding an upper version bound.
- Update retained core samples and tests to exercise the Microsoft.Extensions
  10.0 dependency baseline.
- Re-evaluate the `TestLoggerFactory` mixed-version guard against the supported
  10.0-and-later dependency graph. Remove it only if the supported graph can no
  longer produce the mismatch it protects against.

**Done when**

- The core packages build as `netstandard2.0` with Microsoft.Extensions 10.0
  references and their .NET 10 test suite passes.
- Package metadata and documentation describe .NET Framework 4.7.2 as
  best-effort compatibility for the core packages, without an active CI claim.

### 3. Establish the .NET 10 build baseline

**Scope**

- Update `.github/workflows/build.yml` from `9.0.x` to the selected .NET 10
  SDK, and ensure all restore, build, test, and pack jobs use it.
- Change every retained `net9.0` project in `test` and
  `samples/current`, `samples/legacy`, `samples/xunit-2-latest`, and
  `samples/xunit-3` to `net10.0`.
- Update solution files, scripts, and CI job names so they refer only to the
  retained .NET 10 projects.

**Done when**

- A clean checkout builds, tests, and packs with the .NET 10 SDK on the
  supported CI platforms.
- No retained project, workflow, sample, or user-facing documentation claims
  `.NET 9` support.

### 4. Modernize the ASP.NET Core helper packages

**Scope**

- Redesign `MELT.AspNetCore` and `MELT.Serilog.AspNetCore` for `net10.0`.
- Replace the `Microsoft.AspNetCore.Hosting`, `Microsoft.AspNetCore.Mvc.Testing`,
  and dependency-injection references pinned at `2.1.0` with their .NET 10
  equivalents, using `10.0.0` as the minimum package version without an upper
  version bound. Prefer framework references where appropriate and avoid
  retaining obsolete transitive dependencies.
- Update the helper APIs only where the modern ASP.NET Core testing APIs require
  it, recording user-visible breaks in the migration guide.
- Verify the published package dependency graph contains no ASP.NET Core 2.1,
  `Newtonsoft.Json 9.0.1`, `System.IO.Pipelines 4.5.0`, or
  `System.Text.Encodings.Web 4.5.0` paths.

**Done when**

- The three .NET 10 integration sample variants (standard logging, NLog, and
  Serilog) pass against packages produced from this branch.
- Package vulnerability scanning and `dotnet list package --vulnerable` report
  no known vulnerabilities in the v2 helper-package dependency graph.

### 5. Refresh development-only dependencies

**Scope**

- Upgrade `Microsoft.NET.Test.Sdk` from `17.14.1` to `18.8.1`.
- Upgrade `coverlet.collector` from `6.0.4` to `10.0.1`.
- Upgrade xUnit v2 test dependencies from `2.5.3` to `2.9.3` and
  `xunit.runner.visualstudio` from `2.4.3` to `3.1.5`.
- Upgrade xUnit v3 packages from `3.0.1` to `3.2.2`.
- Upgrade `Microsoft.SourceLink.GitHub` from `8.0.0` to `10.0.301`.

**Done when**

- This work lands as a separate, reviewable change after the target-framework
  migration, with the full test suite passing.
- Production package floors are reviewed deliberately rather than upgraded
  merely because a build-only dependency changed.

### 6. Update package metadata, documentation, and migration guidance

**Scope**

- Update package descriptions, tags, README content, documentation, and sample
  labels to distinguish the `netstandard2.0` core packages from the .NET 10
  ASP.NET Core helpers and test matrix.
- Document Microsoft.Extensions.Logging 10.0 as the core-package minimum and
  .NET Framework 4.7.2 as best-effort compatibility without CI coverage.
- Remove wording that calls ASP.NET Core 2.1 LTS or treats .NET 9 as current.
- Add a v2 migration guide covering the Microsoft.Extensions 10.0 minimum,
  replacement of legacy ASP.NET Core helper dependencies, and the v1 fallback
  for unsupported applications.
- Draft release notes that explain the support-policy rationale for the
  Microsoft.Extensions 10.0 minimum. Link to the [Microsoft .NET Support
  Policy](https://dotnet.microsoft.com/platform/support/policy/dotnet-core),
  explain that Microsoft.Extensions packages are .NET Platform Extensions, and
  state that the .NET 8 LTS and .NET 9 STS support windows end on November 10,
  2026, while .NET 10 LTS is supported through November 14, 2028. Clarify that
  the v2 floor defines MELT's active support policy; it does not assert that an
  older Microsoft.Extensions package stops functioning on that date.
- Confirm NuGet package READMEs and release notes make the breaking support
  policy visible before installation.

**Done when**

- A consumer can tell from the repository and package metadata whether to use
  v2 or the 1.x legacy line without consulting source code.
- The v2 release notes link the Microsoft.Extensions 10.0 minimum to the .NET
  support lifecycle and direct consumers on older dependency lines to 1.x.

### 7. Add ongoing maintenance automation

**Scope**

- Add a monthly Dependabot configuration for `nuget` and `github-actions`.
- Group related development dependencies where that keeps update pull requests
  reviewable, and keep runtime package updates separately visible.
- Decide whether deterministic restore or dependency lock files are appropriate
  for this repository; document and implement the decision rather than adding
  lock files by default.
- Ensure CI validates Dependabot updates using the same .NET 10 SDK
  configuration and test matrix as normal pull requests.

**Done when**

- Dependabot opens monthly update pull requests for both NuGet packages and
  GitHub Actions.
- CI has an explicit, reproducible .NET 10 SDK-selection policy.

## Release gates

- All packages build and pack with the .NET 10 SDK selected by CI.
- Unit tests and all retained current/legacy/xUnit sample tests pass on CI.
- The ASP.NET Core helper packages are tested through the standard, NLog, and
  Serilog integration samples.
- Published package assets and dependency graphs are reviewed before release;
  core packages retain `netstandard2.0` with Microsoft.Extensions 10.0 as their
  minimum, and no package references ASP.NET Core 2.1.
- No `net472` package target or Windows CI test is added without a new decision
  to make full-framework compatibility actively supported.
- README, package metadata, migration guide, and release notes state the v2
  breaking support policy and its Microsoft support-lifecycle rationale.

## Proposed delivery order

1. Retire the ASP.NET Core 2.1 assets and remove any now-unnecessary CI filter.
2. Establish the `netstandard2.0` core package and Microsoft.Extensions 10.0
  baseline.
3. Establish the .NET 10 SDK and target-framework baseline for tests and samples.
4. Modernize the ASP.NET Core helper packages and integration samples.
5. Refresh build-only packages in a dedicated pull request.
6. Complete documentation and migration guidance.
7. Add Dependabot, run release-gate validation, and publish v2.
