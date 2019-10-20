using System.Collections.Generic;

namespace MELT
{
    public class Scope : IScope
    {
        private readonly object _scope;

        public Scope(object scope)
        {
            _scope = scope;
        }

        public string Message => _scope.ToString();
        public IEnumerable<KeyValuePair<string, object>> Properties => _scope as IEnumerable<KeyValuePair<string, object>>;

        public override string ToString() => Message;
    }

    public class BeginScope : IScope
    {
        private readonly BeginScopeContext _scope;

        public BeginScope(BeginScopeContext scope)
        {
            _scope = scope;
        }

        public string LoggerName => _scope.LoggerName;
        public string Message => _scope.Scope.ToString();
        public IEnumerable<KeyValuePair<string, object>> Properties => _scope.Scope as IEnumerable<KeyValuePair<string, object>>;

        public override string ToString() => Message;
    }
}
