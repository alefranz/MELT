using System.Collections.Generic;

namespace MELT
{
    public class Scope
    {
        private readonly object _scope;

        public Scope(object scope)
        {
            _scope = scope;
        }

        public string Message => _scope.ToString();
        public IEnumerable<KeyValuePair<string, object>> Properties => _scope as IEnumerable<KeyValuePair<string, object>>;

        public override string ToString() => _scope.ToString();
    }
}
