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
        /// <summary>
        /// The <see cref="ITestSink"/> instance used for logging.
        /// </summary>
        [Obsolete("This field is obsolete. Use the Sink property instead.")]
        public readonly ITestSink _sink;

        // TODO: keep as internal for testing
        /// <summary>
        /// Initializes a new instance of the <see cref="TestLoggerProvider"/> class with the specified <see cref="ITestSink"/>.
        /// </summary>
        /// <param name="sink">The <see cref="ITestSink"/> to use for logging.</param>
        /// <remarks>
        /// This constructor is obsolete. Use the parameterless constructor instead.
        /// </remarks>
        [Obsolete("This constructor is obsolete. Use the parameterless constructor instead.")]
        public TestLoggerProvider(ITestSink sink)
        {
            _sink = sink;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TestLoggerProvider"/> class with a default <see cref="ITestLoggerSink"/>.
        /// </summary>
        public TestLoggerProvider()
        {
#pragma warning disable CS0612 // Type or member is obsolete
            _sink = new TestSink();
#pragma warning restore CS0612 // Type or member is obsolete
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
