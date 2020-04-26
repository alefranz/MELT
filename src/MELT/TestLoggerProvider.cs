// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.Extensions.Logging;

namespace MELT
{
    public class TestLoggerProvider : ILoggerProvider
    {
        private readonly ITestSink _sink;
        private readonly bool _useScopeFromProperties;

        public TestLoggerProvider(ITestSink sink, bool useScopeFromProperties = false)
        {
            _sink = sink;
            _useScopeFromProperties = useScopeFromProperties;
        }

        public ILogger CreateLogger(string categoryName)
        {
            return new TestLogger(categoryName, _sink, _useScopeFromProperties);
        }

        public void Dispose()
        {
        }
    }
}
