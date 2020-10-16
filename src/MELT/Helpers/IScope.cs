using System.Collections.Generic;

namespace MELT
{
    /// <summary>
    /// Represent a type that holds a captured scope.
    /// </summary>
    public interface IScope
    {
        /// <summary>
        /// The formatted message of the scope captured.
        /// </summary>
        string? Message { get; }

        /// <summary>
        /// A list of <see cref="KeyValuePair{TKey, TValue}" /> that represents the properties of the captured scope.
        /// </summary>
        /// <remarks> Properties are in order and multiple properties can have the same key.</remarks>
        IEnumerable<KeyValuePair<string, object>> Properties { get; }
    }
}
