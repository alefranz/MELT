using System.Collections.Generic;

namespace MELT
{
    public interface IScope
    {
        string? Message { get; }
        IEnumerable<KeyValuePair<string, object>> Properties { get; }
    }
}
