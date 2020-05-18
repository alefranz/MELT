// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.Extensions.Logging;

namespace MELT
{
    [ProviderAlias("TestLogger")]
    public class TestLoggerProvider : ITestLoggerProvider
    {
        public readonly ITestSink _sink;

        // for testing
        internal TestLoggerProvider(ITestSink sink)
        {
            _sink = sink;
        }

        public TestLoggerProvider()
        {
            _sink = new TestSink();
        }

        public ITestLoggerSink Sink => _sink;

        public ILogger CreateLogger(string categoryName)
        {
            return new TestLogger(categoryName, _sink);
        }

        public void Dispose()
        {
        }
    }

    public interface ITestLoggerProvider : ILoggerProvider
    {
        ITestLoggerSink Sink { get; }
    }
}
