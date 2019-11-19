// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace MELT
{
    public readonly struct BeginScopeContext
    {
        internal BeginScopeContext(string loggerName, object? scope)
        {
            LoggerName = loggerName;
            Scope = scope;
        }

        public object? Scope { get; }

        public string LoggerName { get; }
    }
}
