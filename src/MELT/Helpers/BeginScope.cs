using System.Collections.Generic;

namespace MELT
{
    /// <summary>
    /// A captured scope.
    /// </summary>
    public class BeginScope : IScope
    {
        private readonly object? _scope;
        private string? _format;

        internal BeginScope(string loggerName, object? scope)
        {
            LoggerName = loggerName;
            _scope = scope;
        }

        /// <summary>
        /// Gets the name of the logger associated with this scope.
        /// </summary>
        public string? LoggerName { get; }

        /// <inheritdoc/>
        public string? Message => _scope?.ToString();

        /// <inheritdoc/>
        public IEnumerable<KeyValuePair<string, object>> Properties => _scope as IEnumerable<KeyValuePair<string, object>> ?? Constants.EmptyProperties;

        /// <summary>
        /// The original format of the message for this log entry.
        /// </summary>
        public string OriginalFormat => _format ??= Properties.GetOriginalFormat();

        /// <summary>
        /// Return the <see cref="Message"/> as string representation of this scope.
        /// </summary>
        /// <returns>The <see cref="Message"/></returns>
        public override string? ToString() => Message;
    }
}
