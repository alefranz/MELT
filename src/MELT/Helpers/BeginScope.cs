using System;
using System.Collections.Generic;

namespace MELT
{
    /// <summary>
    /// A captured scope.
    /// </summary>
    public class BeginScope : IScope
    {
#pragma warning disable CS0612 // Type or member is obsolete
        private readonly BeginScopeContext _scope;
#pragma warning restore CS0612 // Type or member is obsolete
        private string? _format;
        private static readonly KeyValuePair<string, object>[] _emptyProperties = new KeyValuePair<string, object>[0];

#pragma warning disable CS0612 // Type or member is obsolete
        internal BeginScope(BeginScopeContext scope)
#pragma warning restore CS0612 // Type or member is obsolete
        {
            _scope = scope;
        }

        /// <summary>
        ///
        /// </summary>
        public string? LoggerName => _scope.LoggerName;

        /// <inheritdoc/>
        public string? Message => _scope.Scope?.ToString();

        /// <inheritdoc/>
        public IEnumerable<KeyValuePair<string, object>> Properties => _scope.Scope as IEnumerable<KeyValuePair<string, object>> ?? _emptyProperties;

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
