// Copyright(c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using Xunit;

namespace MELT.Xunit
{
    /// <summary>
    /// Provides assertion methods for verifying logging values.
    /// </summary>
    [Obsolete("The recommended alternative is Xunit." + nameof(LoggingAssert) + ".")]
    public static class LogValuesAssert
    {
        /// <summary>
        /// Asserts that the given key and value are present in the actual values.
        /// </summary>
        /// <param name="key">The key of the item to be found.</param>
        /// <param name="value">The value of the item to be found.</param>
        /// <param name="actualValues">The actual values.</param>
        [Obsolete("The recommended alternative is " + nameof(LoggingAssert) + "." + nameof(Contains) + "(string, object, IEnumerable<KeyValuePair<string, object>>).")]
        public static void Contains(
            string key,
            object value,
            IEnumerable<KeyValuePair<string, object>> actualValues)
            => LoggingAssert.Contains(key, value, actualValues);

        /// <summary>
        /// Asserts that all the expected values are present in the actual values by ignoring
        /// the order of values.
        /// </summary>
        /// <param name="expectedValues">Expected subset of values</param>
        /// <param name="actualValues">Actual set of values</param>
        [Obsolete("The recommended alternative is " + nameof(LoggingAssert) + "." + nameof(Contains) + "(IEnumerable<KeyValuePair<string, object>>, IEnumerable<KeyValuePair<string, object>>).")]
        public static void Contains(
            IEnumerable<KeyValuePair<string, object>> expectedValues,
            IEnumerable<KeyValuePair<string, object>> actualValues)
            => LoggingAssert.Contains(expectedValues, actualValues);

        /// <summary>
        /// Asserts that the given key and value are present in the log entry properties.
        /// </summary>
        /// <param name="key">The key of the item to be found.</param>
        /// <param name="value">The value of the item to be found.</param>
        /// <param name="logEntry">The log entry.</param>
        [Obsolete("The recommended alternative is " + nameof(LoggingAssert) + "." + nameof(Contains) + "(string, object, IEnumerable<KeyValuePair<string, object>>) passing log.Properties.")]
        public static void Contains(
            string key,
            object value,
            LogEntry logEntry)
        {
            Contains(key, value, logEntry.Properties);
        }

        /// <summary>
        /// Asserts that all the expected values are present in the log entry properties by ignoring
        /// the order of values.
        /// </summary>
        /// <param name="expectedValues">Expected subset of values</param>
        /// <param name="logEntry">The log entry.</param>
        [Obsolete("The recommended alternative is " + nameof(LoggingAssert) + "." + nameof(Contains) + "(IEnumerable<KeyValuePair<string, object>>, IEnumerable<KeyValuePair<string, object>>) passing log.Properties.")]
        public static void Contains(
            IEnumerable<KeyValuePair<string, object>> expectedValues,
            LogEntry logEntry)
        {
            Contains(expectedValues, logEntry.Properties);
        }

        /// <summary>
        /// Asserts that the given key and value are present in the scope properties.
        /// </summary>
        /// <param name="key">The key of the item to be found.</param>
        /// <param name="value">The value of the item to be found.</param>
        /// <param name="scope">The scope.</param>
        [Obsolete("The recommended alternative is " + nameof(LoggingAssert) + "." + nameof(Contains) + "(string, object, IEnumerable<KeyValuePair<string, object>>) passing scope.Properties.")]
        public static void Contains(
            string key,
            object value,
            IScope scope)
        {
            Contains(key, value, scope.Properties);
        }

        /// <summary>
        /// Asserts that all the expected values are present in the scope properties by ignoring
        /// the order of values.
        /// </summary>
        /// <param name="expectedValues">Expected subset of values</param>
        /// <param name="scope">The scope.</param>
        [Obsolete("The recommended alternative is " + nameof(LoggingAssert) + "." + nameof(Contains) + "(IEnumerable<KeyValuePair<string, object>>, IEnumerable<KeyValuePair<string, object>>) passing scope.Properties.")]
        public static void Contains(
            IEnumerable<KeyValuePair<string, object>> expectedValues,
            IScope scope)
        {
            Contains(expectedValues, scope.Properties);
        }
    }
}
