using System.Collections.Generic;

namespace MELT
{
    public class Scope : IScope
    {
        private readonly object? _scope;

        public Scope(object? scope)
        {
            _scope = scope;
        }

        public string Message => _scope?.ToString() ?? string.Empty;
        public IReadOnlyList<KeyValuePair<string, object>> Properties => _scope as IReadOnlyList<KeyValuePair<string, object>> ?? Constants.EmptyProperties;

        public override string ToString() => Message;
    }
}
