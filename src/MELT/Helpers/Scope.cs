using System.Collections.Generic;

namespace MELT
{
    /// <summary>
    /// The scope closer to a log entry.
    /// </summary>
    public class Scope : IScope
    {
        private readonly object? _scope;
        private string? _format;

        internal Scope(object? scope)
        {
            _scope = scope;
        }

        /// <inheritdoc/>
        public string Message => _scope?.ToString() ?? string.Empty;

        /// <inheritdoc/>
        public IReadOnlyList<KeyValuePair<string, object>> Properties => _scope as IReadOnlyList<KeyValuePair<string, object>> ?? Constants.EmptyProperties;

        /// <summary>
        /// The original format of the message for this log entry.
        /// </summary>
        public string OriginalFormat => _format ??= Properties.GetOriginalFormat();

        /// <summary>
        /// Return the <see cref="Message"/> as string representation of this scope.
        /// </summary>
        /// <returns>The <see cref="Message"/></returns>
        public override string ToString() => Message;
    }
}
