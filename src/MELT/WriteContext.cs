// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace MELT
{
    public readonly struct WriteContext
    {
        public WriteContext(LogLevel logLevel, EventId eventId, object? state, Exception? exception, IEnumerable<object> scopes, string loggerName, string? message)
        {
            LogLevel = logLevel;
            EventId = eventId;
            State = state;
            Exception = exception;
            Scopes = scopes;
            LoggerName = loggerName;
            Message = message;
        }

        public LogLevel LogLevel { get; }

        public EventId EventId { get; }

        public object? State { get; }

        public Exception? Exception { get; }

        public IEnumerable<object> Scopes { get; }

        public string LoggerName { get; }

        public string? Message { get; }
    }
}
