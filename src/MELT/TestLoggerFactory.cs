// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace MELT
{
    public class TestLoggerFactory : LoggerFactory, ITestLoggerFactory
    {
        private readonly ITestSink _sink;

        internal TestLoggerFactory(ITestSink sink)
        {
            _sink = sink;
            //AddProvider(new TestLoggerProvider(_sink));
        }

        [Obsolete]
        public IEnumerable<LogEntry> LogEntries => _sink.LogEntries;

        [Obsolete]
        public IEnumerable<BeginScope> Scopes => _sink.Scopes;

        public ITestLoggerSink Sink => _sink;

        //public ILogger CreateLogger(string name)
        //{
        //    return new TestLogger(name, _sink);
        //}

        //public void AddProvider(ILoggerProvider provider)
        //{
        //    // no op
        //}

        //public void Dispose()
        //{
        //    // no op
        //}
    }
}
