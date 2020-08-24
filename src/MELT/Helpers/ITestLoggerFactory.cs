using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace MELT
{
    /// <summary>
    /// Represent a type used to get access to the <see cref="ITestLoggerSink" /> containing the captured logs and create instances of <see cref="ILogger" /> that capture logs and scopes for testing.
    /// </summary>
    public interface ITestLoggerFactory : ILoggerFactory
    {
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        [Obsolete("The recommended alternative is " + nameof(Sink) + "." + nameof(ITestLoggerSink.LogEntries) + ".")]
        IEnumerable<LogEntry> LogEntries { get; }

        [Obsolete("The recommended alternative is " + nameof(Sink) + "." + nameof(ITestLoggerSink.Scopes) + ".")]
        IEnumerable<BeginScope> Scopes { get; }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

        /// <summary>
        /// The <see cref="ITestLoggerSink"/> which gives access to the captured logs.
        /// </summary>
        ITestLoggerSink Sink { get; }
    }
}
