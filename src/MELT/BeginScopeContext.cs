// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace MELT
{
    [Obsolete]
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public readonly struct BeginScopeContext
    {
        public BeginScopeContext(string loggerName, object? scope)
        {
            LoggerName = loggerName;
            Scope = scope;
        }

        public object? Scope { get; }

        public string LoggerName { get; }
    }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}
