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
        private readonly TestLoggerSink _sink;

        /// <summary>
        /// Initializes a new instance of the <see cref="TestLoggerProvider"/> class with a default <see cref="ITestLoggerSink"/>.
        /// </summary>
        public TestLoggerProvider() : this(new TestLoggerSink())
        {
        }

        internal TestLoggerProvider(TestLoggerSink sink)
        {
            _sink = sink ?? throw new ArgumentNullException(nameof(sink));
        }

        /// <inheritdoc/>
        public ITestLoggerSink Sink => _sink;

        /// <inheritdoc/>
        public ILogger CreateLogger(string categoryName)
        {
            return new TestLogger(categoryName, _sink);
        }

        /// <inheritdoc/>
        public void Dispose()
        {
        }
    }
}
