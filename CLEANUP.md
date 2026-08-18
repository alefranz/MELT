# MELT v2 cleanup

This document tracks compatibility code and obsolete APIs to remove before the
MELT v2 release. Because these changes break source and binary compatibility,
v2 is the right boundary for removing them rather than carrying them into
another major release.

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
