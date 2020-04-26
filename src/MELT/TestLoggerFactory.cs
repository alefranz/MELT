// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.Extensions.Logging;
using System.Collections.Generic;

namespace MELT
{
    public class TestLoggerFactory : ITestLoggerFactory
    {
        private readonly ITestSink _sink;
        private readonly bool _useScopeFromProperties;

        public TestLoggerFactory(ITestSink sink, bool useScopeFromProperties = false)
        {
            _sink = sink;
            _useScopeFromProperties = useScopeFromProperties;
        }

        public IEnumerable<LogEntry> LogEntries => _sink.LogEntries;
        public IEnumerable<BeginScope> Scopes => _sink.Scopes;

        public ILogger CreateLogger(string name)
        {
            return new TestLogger(name, _sink, _useScopeFromProperties);
        }

        public void AddProvider(ILoggerProvider provider)
        {
            // no op
        }

        public void Dispose()
        {
            // no op
        }
    }
}
