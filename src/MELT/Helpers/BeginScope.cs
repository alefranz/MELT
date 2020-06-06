using System;
using System.Collections.Generic;

namespace MELT
{
    public class BeginScope : IScope
    {
        private readonly BeginScopeContext _scope;
        private string? _format;
        private static readonly KeyValuePair<string, object>[] _emptyProperties = new KeyValuePair<string, object>[0];

        internal BeginScope(BeginScopeContext scope)
        {
            _scope = scope;
        }

        public string? LoggerName => _scope.LoggerName;
        public string? Message => _scope.Scope?.ToString();
        public IReadOnlyList<KeyValuePair<string, object>> Properties => _scope.Scope as IReadOnlyList<KeyValuePair<string, object>> ?? _emptyProperties;
        public string OriginalFormat => _format ??= Properties.GetOriginalFormat();
        public override string? ToString() => Message;
    }
}
