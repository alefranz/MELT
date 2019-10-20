// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;

namespace MELT
{
    public class TestLoggerFactory : ILoggerFactory
    {
        private readonly ITestSink _sink;
        private readonly bool _enabled;

        public TestLoggerFactory() : this(new TestSink(), true)
        {
        }

        public TestLoggerFactory(ITestSink sink, bool enabled)
        {
            _sink = sink;
            _enabled = enabled;
        }

        public IEnumerable<LogEntry> LogEntries => _sink.Writes.Select(x => new LogEntry(x));
        public IEnumerable<Scope> Scopes => _sink.BeginScopes.Select(x => new Scope(x.Scope));

        public ILogger CreateLogger(string name)
        {
            return new TestLogger(name, _sink, _enabled);
        }

        public void AddProvider(ILoggerProvider provider)
        {
        }

        public void Dispose()
        {
        }
    }
}
