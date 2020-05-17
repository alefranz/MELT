// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.Extensions.Logging;

namespace MELT
{
    [ProviderAlias("TestLogger")]
    public class TestLoggerProvider : ILoggerProvider
    {
        public readonly ITestSink _sink;

        internal TestLoggerProvider(ITestSink sink)
        {
            _sink = sink;
        }

        public ILogger CreateLogger(string categoryName)
        {
            return new TestLogger(categoryName, _sink);
        }

        public void Dispose()
        {
        }
    }
}
