// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;

namespace MELT
{
    public class TestLoggerFactory : ITestLoggerFactory
    {
        private readonly ITestSink _sink;

        public TestLoggerFactory(ITestSink sink)
        {
            _sink = sink;
        }

        public IEnumerable<LogEntry> LogEntries => _sink.Writes.Select(x => new LogEntry(x));
        public IEnumerable<Scope> Scopes => _sink.BeginScopes.Select(x => new Scope(x.Scope));

        public ILogger CreateLogger(string name)
        {
            return new TestLogger(name, _sink);
        }

        public void AddProvider(ILoggerProvider provider)
        {
        }

        public void Dispose()
        {
        }
    }
}
