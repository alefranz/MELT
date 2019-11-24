// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace MELT
{
    internal class TestScope : IDisposable
    {
        public static TestScope Instance { get; } = new TestScope();

        private TestScope()
        {
        }

        /// <inheritdoc />
        public void Dispose()
        {
        }
    }

}
