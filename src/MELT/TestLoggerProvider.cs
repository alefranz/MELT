// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.Extensions.Logging;

namespace MELT
{
    /// <inheritdoc/>
    [ProviderAlias("TestLogger")]
    public class TestLoggerProvider : ITestLoggerProvider
    {
        public readonly ITestSink _sink;

        // TODO: keep as internal for testing
        [Obsolete]
        public TestLoggerProvider(ITestSink sink)
        {
            _sink = sink;
        }

        public TestLoggerProvider()
        {
            _sink = new TestSink();
        }

        /// <inheritdoc/>
        public ITestLoggerSink Sink => _sink;

        public ILogger CreateLogger(string categoryName)
        {
            return new TestLogger(categoryName, _sink);
        }

        public void Dispose()
        {
        }
    }
}
