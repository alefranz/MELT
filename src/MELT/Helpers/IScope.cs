using System.Collections.Generic;

namespace MELT
{
    public interface IScope
    {
        string? Message { get; }
        IReadOnlyList<KeyValuePair<string, object>> Properties { get; }
    }
}
