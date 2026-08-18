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
- Build the core baseline and dedicated samples on .NET 8, while keeping
  default samples and core test suites on .NET 10. Retain named .NET 9 web
  sample variants only where they validate framework-specific behavior.
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
- Keep `xunit.v3.assert` 3.0.1 as the `MELT.Xunit.v3` compatibility floor and
  validate the package against both xUnit.net v3 framework 3.x and 4.x.

## Release backlog

### 1. Establish the SDK and runtime validation matrix - Completed

**Scope**

- Update `.github/workflows/build.yml` to install the .NET 8, .NET 9, and .NET
  10 SDKs. Use the .NET 8 target as the minimum core baseline, .NET 9 for
  explicit compatibility samples, and .NET 10 for the default solution build,
  test, and pack validation.
- Move generic retained `net9.0` projects in `test`, `samples/current`,
  `samples/legacy`, `samples/xunit-2-latest`, and `samples/xunit-3` to
  `net8.0` or `net10.0` as appropriate. Keep .NET 9 only in clearly named web
  compatibility sample and integration-test pairs.
- Keep dedicated .NET 8 samples for the core dependency baseline, and retain
  .NET 8 / .NET 9 web sample pairs beside the default .NET 10 web samples.
- Update the build script and CI job names to describe the combined .NET 8,
  .NET 9, and .NET 10 validation matrix.
- Schedule a post-November maintenance item to retire the .NET 8 and .NET 9
  active test lanes and make .NET 10 the minimum active-support baseline.

**Done when**

- CI installs the .NET 8, .NET 9, and .NET 10 SDKs, then builds and tests the
  complete solution and packs it on push.
- The named .NET 8, .NET 9, and .NET 10 web sample variants each run their
  integration tests on the corresponding runtime.
- No project, workflow, sample, or user-facing documentation presents .NET 9
  as the default or sole current target.

### 2. Validate the modern ASP.NET Core helper packages - Completed

**Scope**

- Update the helper APIs only where the modern ASP.NET Core testing APIs require
  it, recording user-visible breaks in the migration guide.
- Verify the published package dependency graph contains no ASP.NET Core 2.1,
  `Newtonsoft.Json 9.0.1`, `System.IO.Pipelines 4.5.0`, or
  `System.Text.Encodings.Web 4.5.0` paths.

**Done when**

- The standard logging integration sample passes on .NET 8, .NET 9, and .NET
  10, while NLog and Serilog integration samples pass on .NET 8 and .NET 10
  against packages produced from this branch.
- Package vulnerability scanning and `dotnet list package --vulnerable` report
  no known vulnerabilities in the v2 helper-package dependency graph.

### 3. Refresh development-only dependencies - Completed

**Scope**

- Upgrade `Microsoft.NET.Test.Sdk`, `coverlet.collector`, xUnit v2/v3, their
  runners, and `Microsoft.SourceLink.GitHub` to their selected stable versions
  after confirming .NET 8 and .NET 10 compatibility.
- Keep the development-dependency refresh in a separate, reviewable change
  after the target-framework migration.
- Review production package floors deliberately rather than upgrading them
  merely because a build-only dependency changed.

**Done when**

- The full test suite passes under the combined validation matrix.
- Production package floors are documented separately from build-only package
  updates.

### 4. Update package metadata, documentation, and migration guidance - Completed

**Scope**

- Update package descriptions, tags, README content, documentation, and sample
  labels to distinguish the `netstandard2.0` core packages, the dedicated .NET
  8 baseline samples, the default .NET 10 samples, and the named .NET 9 web
  compatibility variants.
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

### 5. Align maintenance automation with the runtime lanes - Completed

**Scope**

- Ensure CI validates Dependabot updates using the same combined validation
  matrix as normal pull requests.

**Done when**

- CI has an explicit, reproducible SDK and runtime selection policy for the
  active support matrix.

**Completed**

- The standard pull-request trigger, including Dependabot pull requests, runs
  one named validation job for the .NET 8, .NET 9, and .NET 10 matrix.
- A shared workflow action installs and reports the SDKs and runtimes used by
  validation, pack, and publish jobs so those lanes cannot diverge.

### 6. Make the tag publishing job resolve repository-local actions - In progress

**Scope**

- Check out the repository before the `publish` job invokes
  `./.github/actions/setup-melt-dotnet`, or replace that invocation with setup
  that does not depend on files in the workspace.
- Validate the complete tag path from SDK setup through artifact download and
  package push without publishing a real release.

**Done when**

- A tag-triggered workflow can resolve every action used by the `publish` job
  and reach the NuGet push step with downloaded package artifacts.

**Completed**

- The `publish` job checks out the repository before invoking the local .NET
  setup action, allowing tag-triggered runs to continue through artifact
  download to the NuGet push step.
- The publish command targets the `.nupkg` files downloaded directly into
  `./artifacts`, without relying on Bash `globstar`.

**Remaining**

- Run the `Build` workflow manually from the v2 branch with **dry_run_publish**
  enabled. It packs and consumer-tests the packages, then pushes them to a
  runner-local NuGet feed without publishing a real release.

### 7. Test the produced packages as consumer dependencies - Completed

**Scope**

- Add a post-pack smoke test that restores representative .NET 8, .NET 9, and
  .NET 10 consumers from the packages in the workflow artifact rather than
  from project references.
- Verify that NuGet selects the expected `netstandard2.0`, `net8.0`, and
  `net10.0` assets and resolves the intended minimum dependency graph.

**Done when**

- The six v2 packages can be installed together in representative consumers,
  and the core, ASP.NET Core, Serilog, and xUnit entry points build and run
  from the packed artifacts.

**Completed**

- The post-pack `consumer-smoke` workflow job downloads the package artifact
  and restores executable .NET 8, .NET 9, and .NET 10 consumers with the
  artifact directory as an additional NuGet source.
- The .NET 8 and .NET 10 consumers install all six packages; the .NET 9
  consumer validates the four `netstandard2.0` packages because the ASP.NET
  Core helpers deliberately target only .NET 8 and .NET 10. The smoke script
  verifies the selected compile assets and runs the core, Serilog, and xUnit
  entry points.

### 8. Complete an auditable transitive vulnerability gate - Completed

**Scope**

- Run `dotnet list MELT.sln package --vulnerable --include-transitive` against
  the final restored dependency graph and retain the result with the release
  validation.
- Ensure the gate fails or otherwise blocks release when a vulnerable direct
  or transitive dependency is reported; the current default restore audit is
  configured for direct dependencies only.

**Done when**

- The final v2 package and sample dependency graphs report no known direct or
  transitive vulnerabilities, and the result is reproducible during release
  validation.

**Completed**

- The pack job scans the final restored `MELT.sln` graph with `dotnet list
  package --vulnerable --include-transitive`, writes its JSON result to
  `artifacts/vulnerability-report.json`, and uploads it with the packed
  packages.
- The scan script fails the pack job if the command cannot complete, the JSON
  report contains an error, or any direct or transitive package includes a
  vulnerability advisory.

### 9. Validate xUnit.net v3 framework 4.x compatibility - Completed

**Scope**

- Retain the xUnit.net v3 framework 3.2.2 sample as the compatibility floor
  validation without raising `MELT.Xunit.v3`'s `xunit.v3.assert` dependency.
- Add an xUnit.net v3 framework 4.0.0 sample that runs the same MELT assertions
  under the repository's VSTest-based .NET 10 test flow.
- Document compatibility with both framework 3.x and 4.x.

**Done when**

- Both xUnit sample projects run as part of solution validation and exercise
  the same MELT logging assertions.

**Completed**

- `SampleLibraryX3.Tests` continues to test xUnit.net v3 framework 3.2.2.
- `SampleLibraryXunit4.Tests` links the same test sources and passes all 13
  tests with `xunit.v3.mtp-off` 4.0.0, the xUnit-provided package for retaining
  VSTest execution on .NET 10.

## Post-November 2026 maintenance

### Retire the .NET 8 and .NET 9 active validation lanes

**Scope**

- After November 10, 2026, remove .NET 8 and .NET 9 from the active validation
  matrix, make .NET 10 the minimum active-support baseline, and update the
  support-policy documentation.

**Done when**

- CI and its documentation name .NET 10 and later as the active support
  matrix, without presenting .NET 8 or .NET 9 as actively supported.

## Release gates

- Core packages and dedicated .NET 8 samples build and run targeting .NET 8;
  the default solution builds, tests, and packs targeting .NET 10.
- Unit tests and all retained current/legacy/xUnit sample tests pass on CI.
- The shared xUnit sample assertions pass with xUnit.net v3 framework 3.x and
  4.x without raising the `MELT.Xunit.v3` package's 3.0.1 dependency floor.
- The ASP.NET Core helper packages are tested through the standard, NLog, and
  Serilog integration samples on .NET 8 and .NET 10, with the named standard
  web compatibility variant also tested on .NET 9.
- Published package assets and dependency graphs are reviewed before release;
  core packages retain `netstandard2.0` with Microsoft.Extensions 8.0 as their
  minimum, and no package references ASP.NET Core 2.1.
- No `net472` package target or Windows CI test is added without a new decision
  to make full-framework compatibility actively supported.
- README, package metadata, migration guide, and release notes state the v2
  breaking support policy, its initial .NET 8 baseline, and the November 2026
  transition to .NET 10-only active support.

## Proposed delivery order

1. Establish the SDK and runtime validation matrix.
2. Modernize the ASP.NET Core helper packages and integration samples.
3. Refresh build-only packages in a dedicated pull request.
4. Complete documentation and migration guidance.
5. Align maintenance automation with the supported runtime lanes.
6. Repair and validate the tag publishing path.
7. Test the packed artifacts as consumer dependencies.
8. Complete the transitive vulnerability gate.
9. Validate xUnit.net v3 framework 4.x compatibility and publish v2.
