// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace MELT
{
    [Obsolete]
    public readonly struct WriteContext
    {
        public WriteContext(LogLevel logLevel, EventId eventId, object? state, Exception? exception, object? scope,
            string loggerName, string? message, IEnumerable<object?> scopes)
        {
            LogLevel = logLevel;
            EventId = eventId;
            State = state;
            Exception = exception;
            Scope = scope;
            LoggerName = loggerName;
            Message = message;
            Scopes = scopes;
        }

        public LogLevel LogLevel { get; }

        public EventId EventId { get; }

        public object? State { get; }

        public Exception? Exception { get; }

        /// <summary>
        /// The most recent scope to have been created by the logger.
        /// </summary>
        [Obsolete("The preferred alternative is " + nameof(Scopes) + ".")]
        public object? Scope { get; }

        public string LoggerName { get; }

        public string? Message { get; }

        /// <summary>
        /// The full scope when the event was logged.
        /// </summary>
        public IEnumerable<object?> Scopes { get; }
    }
}
