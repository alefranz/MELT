// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.Extensions.Logging;

namespace MELT
{
    /// <summary>
    /// Provides access to the sink that contains the captured logs and scopes.
    /// </summary>
    public interface ITestLoggerProvider : ILoggerProvider
    {
        /// <summary>
        /// The sink that contains the captured logs and scopes.
        /// </summary>
        ITestLoggerSink Sink { get; }
    }
}
