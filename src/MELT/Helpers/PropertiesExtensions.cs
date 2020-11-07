using System.Collections.Generic;
using System.Diagnostics;

namespace MELT
{
    internal static class PropertiesExtensions
    {
        public static string GetOriginalFormat(this IEnumerable<KeyValuePair<string, object>> properties)
        {
            foreach (var prop in properties)
            {
                if (prop.Key == "{OriginalFormat}")
                {
                    if (prop.Value is string value) return value;

                    Debug.Assert(false, "{OriginalFormat} should always be string.");
                    return Constants.NullString;
                }
            }

            Debug.Assert(false, "{OriginalFormat} should always be present.");
            return Constants.NullString;
        }
    }
}
