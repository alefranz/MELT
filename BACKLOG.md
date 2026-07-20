# MELT v2 Backlog

## Release intent

Prepare a breaking MELT v2 release as soon as this modernization work satisfies
the release gates. At release, v2 actively supports applications targeting
.NET 8 and later, with a minimum Microsoft.Extensions.Logging dependency line
of 8.0.0. This is the smallest currently supported .NET baseline and avoids
making users wait for a release that only helps them after .NET 8 has expired.

.NET 8 and .NET 9 both reach end of support on November 10, 2026. At that
point, update v2's active-support and CI policy to .NET 10 and later in a v2
maintenance release; do not defer v2 or require a v3 solely for that support
policy change. A retained 8.0 dependency floor or compatible package asset is
not a promise to support an out-of-support runtime after that date.

The core packages retain broad runtime compatibility through `netstandard2.0`;
.NET Framework 4.7.2 is a best-effort core-package scenario, not an actively
tested support target.

This branch is the integration branch for the release. Keep each backlog item
in a focused pull request so package, build, and compatibility changes can be
reviewed independently.

## Compatibility decisions

- Keep `MELT`, `MELT.Serilog`, `MELT.Xunit`, and `MELT.Xunit.v3` on
  `netstandard2.0`.
- Set the Microsoft.Extensions.Logging, Logging.Abstractions, and dependency
  injection package references used by the core packages to a minimum version
  of `8.0.0`. Do not use version ranges.
- Set the MELT Serilog package references to Serilog `3.1.1` and
  Serilog.Extensions.Logging `8.0.0`. Keep Serilog `3.1.1` as the supported
  minimum while validating the latest supported major line separately.
- Build and test the v2 baseline on .NET 8 and .NET 10. Retained samples and
  tests should target `net8.0` and, where practical, `net10.0`, so the minimum
  and current LTS environments are both exercised.
- Target the ASP.NET Core helper packages for `net8.0` and `net10.0`, with the
  appropriate Microsoft.AspNetCore testing dependency group for each target.
- Treat .NET Framework 4.7.2 as best-effort compatibility for the
  `netstandard2.0` core packages only. Do not add a `net472` target or a
  dedicated Windows CI job.
- Remove all `net481` projects and the ASP.NET Core 2.1 sample instead of
  preserving an untested historical compatibility promise.
- Retire direct package references to ASP.NET Core 2.1. The ASP.NET Core
  helper packages must use the .NET 8 and .NET 10 ASP.NET Core testing stacks.
- Document 1.x as the legacy line for consumers using Microsoft.Extensions
  versions earlier than 8.0 or legacy ASP.NET Core helpers. Do not backport the
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
  dependency-injection references to a minimum version of `8.0.0`, without an
  upper version bound.
- Update retained core samples and tests to exercise the Microsoft.Extensions
  8.0 dependency baseline on .NET 8 and .NET 10.
- Keep explicit Serilog sample variants for the Serilog `3.1.1` minimum and
  Serilog `4.4.0`. Both variants must exercise MELT's Serilog test sink and
  integration assertions.
- Re-evaluate the `TestLoggerFactory` mixed-version guard against the supported
  8.0-and-later dependency graph. Remove it only if the supported graph can no
  longer produce the mismatch it protects against.

**Done when**

- The core packages build as `netstandard2.0` with Microsoft.Extensions 8.0
  references and their .NET 8 and .NET 10 test matrix passes.
- The Serilog `3.1.1` and `4.4.0` integration sample variants pass with
  MELT's Serilog test sink.
- Package metadata and documentation describe .NET Framework 4.7.2 as
  best-effort compatibility for the core packages, without an active CI claim.

### 3. Establish the .NET 8 baseline and .NET 10 validation matrix

**Scope**

- Update `.github/workflows/build.yml` to install supported .NET 8 and .NET 10
  SDKs. Use .NET 8 to establish the minimum baseline and .NET 10 to validate
  current-LTS compatibility.
- Change every retained `net9.0` project in `test` and
  `samples/current`, `samples/legacy`, `samples/xunit-2-latest`, and
  `samples/xunit-3` to `net8.0`; multi-target `net8.0;net10.0` where the
  project can do so without obscuring the sample.
- Update solution files, scripts, and CI job names so they describe the
  .NET 8 / .NET 10 matrix, not .NET 9 as the sole baseline.
- Schedule a small post-November maintenance item to remove .NET 8 from the
  active test matrix and make .NET 10 the minimum active-support baseline.

**Done when**

- A clean checkout builds, tests, and packs on both the .NET 8 and .NET 10 SDKs
  on the supported CI platforms.
- No retained project, workflow, sample, or user-facing documentation presents
  .NET 9 as the sole current target.

### 4. Modernize the ASP.NET Core helper packages

**Scope**

- Redesign `MELT.AspNetCore` and `MELT.Serilog.AspNetCore` for `net8.0` and
  `net10.0`.
- Replace the `Microsoft.AspNetCore.Hosting`, `Microsoft.AspNetCore.Mvc.Testing`,
  and dependency-injection references pinned at `2.1.0` with matching .NET 8
  and .NET 10 equivalents. Prefer framework references where appropriate and
  avoid retaining obsolete transitive dependencies.
- Update the helper APIs only where the modern ASP.NET Core testing APIs require
  it, recording user-visible breaks in the migration guide.
- Verify the published package dependency graph contains no ASP.NET Core 2.1,
  `Newtonsoft.Json 9.0.1`, `System.IO.Pipelines 4.5.0`, or
  `System.Text.Encodings.Web 4.5.0` paths.

**Done when**

- The standard logging, NLog, and Serilog integration sample variants pass on
  both .NET 8 and .NET 10 against packages produced from this branch.
- Package vulnerability scanning and `dotnet list package --vulnerable` report
  no known vulnerabilities in the v2 helper-package dependency graph.

### 5. Refresh development-only dependencies

**Scope**

- Upgrade `Microsoft.NET.Test.Sdk`, `coverlet.collector`, xUnit v2/v3, their
  runners, and `Microsoft.SourceLink.GitHub` to their selected stable versions
  after confirming .NET 8 and .NET 10 compatibility.
- Keep the development-dependency refresh in a separate, reviewable change
  after the target-framework migration.
- Review production package floors deliberately rather than upgrading them
  merely because a build-only dependency changed.

**Done when**

- The full test suite passes on the .NET 8 / .NET 10 matrix.
- Production package floors are documented separately from build-only package
  updates.

### 6. Update package metadata, documentation, and migration guidance

**Scope**

- Update package descriptions, tags, README content, documentation, and sample
  labels to distinguish the `netstandard2.0` core packages from the .NET 8 / .NET
  10 ASP.NET Core helpers and test matrix.
- Document Microsoft.Extensions.Logging 8.0 as the core-package minimum and
  .NET Framework 4.7.2 as best-effort compatibility without CI coverage.
- Remove wording that calls ASP.NET Core 2.1 LTS or treats .NET 9 as the only
  current target.
- Add a v2 migration guide covering the Microsoft.Extensions 8.0 minimum,
  replacement of legacy ASP.NET Core helper dependencies, and the 1.x fallback
  for unsupported applications.
- Draft release notes that explain the support-policy rationale for the initial
  Microsoft.Extensions 8.0 minimum. Link to the [Microsoft .NET Support
  Policy](https://dotnet.microsoft.com/platform/support/policy/dotnet-core),
  state that .NET 8 and .NET 9 are supported through November 10, 2026, and
  that .NET 10 LTS is supported through November 14, 2028. Clarify that after
  November 10 active v2 support moves to .NET 10 and later; the 8.0 floor
  defines initial compatibility and does not assert ongoing support for an
  out-of-support runtime.
- Confirm NuGet package READMEs and release notes make the breaking support
  policy visible before installation.

**Done when**

- A consumer can tell from the repository and package metadata whether to use
  v2 or the 1.x legacy line without consulting source code.
- The v2 release notes link the Microsoft.Extensions 8.0 minimum to the .NET
  support lifecycle and direct consumers on older dependency lines to 1.x.

### 7. Add ongoing maintenance automation

**Scope**

- Add a monthly Dependabot configuration for `nuget` and `github-actions`.
- Group related development dependencies where that keeps update pull requests
  reviewable, and keep runtime package updates separately visible.
- Decide whether deterministic restore or dependency lock files are appropriate
  for this repository; document and implement the decision rather than adding
  lock files by default.
- Ensure CI validates Dependabot updates using the same .NET 8 / .NET 10 SDK
  configuration and test matrix as normal pull requests.

**Done when**

- Dependabot opens monthly update pull requests for both NuGet packages and
  GitHub Actions.
- CI has an explicit, reproducible SDK-selection policy for the active
  support matrix.

## Release gates

- All packages build and pack on the .NET 8 / .NET 10 CI matrix.
- Unit tests and all retained current/legacy/xUnit sample tests pass on CI.
- The ASP.NET Core helper packages are tested through the standard, NLog, and
  Serilog integration samples on both supported target frameworks.
- Published package assets and dependency graphs are reviewed before release;
  core packages retain `netstandard2.0` with Microsoft.Extensions 8.0 as their
  minimum, and no package references ASP.NET Core 2.1.
- No `net472` package target or Windows CI test is added without a new decision
  to make full-framework compatibility actively supported.
- README, package metadata, migration guide, and release notes state the v2
  breaking support policy, its initial .NET 8 baseline, and the November 2026
  transition to .NET 10-only active support.

## Proposed delivery order

1. Retire the ASP.NET Core 2.1 assets and remove any now-unnecessary CI filter.
2. Establish the `netstandard2.0` core package and Microsoft.Extensions 8.0
   baseline.
3. Establish the .NET 8 / .NET 10 SDK and target-framework validation matrix.
4. Modernize the ASP.NET Core helper packages and integration samples.
5. Refresh build-only packages in a dedicated pull request.
6. Complete documentation and migration guidance.
7. Add Dependabot, run release-gate validation, and publish v2.
