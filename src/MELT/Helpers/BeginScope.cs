using System.Collections.Generic;

namespace MELT
{
    public class BeginScope : IScope
    {
        private readonly BeginScopeContext _scope;
        private static readonly KeyValuePair<string, object>[] _emptyProperties = new KeyValuePair<string, object>[0];

        internal BeginScope(BeginScopeContext scope)
        {
            _scope = scope;
        }

        public string? LoggerName => _scope.LoggerName;
        public string? Message => _scope.Scope?.ToString();
        public IReadOnlyList<KeyValuePair<string, object>> Properties => _scope.Scope as IReadOnlyList<KeyValuePair<string, object>> ?? _emptyProperties;

        public override string? ToString() => Message;
    }
}
