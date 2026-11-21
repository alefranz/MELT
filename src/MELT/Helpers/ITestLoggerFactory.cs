using Microsoft.Extensions.Logging;

namespace MELT
{
    /// <summary>
    /// Represent a type used to get access to the <see cref="ITestLoggerSink" /> containing the captured logs and create instances of <see cref="ILogger" /> that capture logs and scopes for testing.
    /// </summary>
    public interface ITestLoggerFactory : ILoggerFactory
    {
        /// <summary>
        /// The <see cref="ITestLoggerSink"/> which gives access to the captured logs.
        /// </summary>
        ITestLoggerSink Sink { get; }
    }
}
