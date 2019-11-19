using System.Collections.Generic;

namespace MELT
{
    public class Scope : IScope
    {
        private readonly object? _scope;
        private static readonly KeyValuePair<string, object>[] _emptyProperties = new KeyValuePair<string, object>[0];

        public Scope(object? scope)
        {
            _scope = scope;
        }

        public string? Message => _scope?.ToString();
        public IEnumerable<KeyValuePair<string, object>> Properties => _scope as IEnumerable<KeyValuePair<string, object>> ?? _emptyProperties;

        public override string? ToString() => Message;
    }
}
