// Copyright(c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using MELT;
using MELT.Serilog;
using Serilog.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit.Sdk;

namespace Xunit
{
    public static class SerilogLogValuesAssert
    {
        //public static ISerilogTestSink AsSerilog(this ITestSink sink)
        //{
        //    return new SerilogTestSink(sink);
        //}

        private static readonly IReadOnlyList<LogEventPropertyValue> _emptyProperties = new LogEventPropertyValue[0];

        public static IReadOnlyList<LogEventPropertyValue> GetSerilogScope(this SerilogLogEntry log)
        {
            var scopeSequence = log.Properties.SingleOrDefault(x => x.Key == "Scope").Value as SequenceValue;
            return scopeSequence?.Elements ?? _emptyProperties;
        }

        //AssertStructuredValue(log, "thing", new[] {
        //        new KeyValuePair<string, object>("foo", "bar"),
        //        new KeyValuePair<string, object>("answer", 42)
        //    });
        public static void AssertStructuredValue(SerilogLogEntry log, string name, IEnumerable<KeyValuePair<string, object>> expectedValues)
        {
            var thing = Assert.Single(log.Properties, x => x.Key == name);
            var value = Assert.IsType<StructureValue>(thing.Value);
            foreach (var expected in expectedValues)
            {
                var property = Assert.Single(value.Properties, x => x.Name == expected.Key);
                Assert.Equal(new ScalarValue(expected.Value), property.Value);
            }
        }

        /// <summary>
        /// Asserts that the given key and value are present in the actual values.
        /// </summary>
        /// <param name="key">The key of the item to be found.</param>
        /// <param name="value">The value of the item to be found.</param>
        /// <param name="actualValues">The actual values.</param>
        public static void Contains(
            string key,
            object value,
            IEnumerable<KeyValuePair<string, object>> actualValues)
        {
            Contains(new[] { new KeyValuePair<string, object>(key, value) }, actualValues);
        }

        /// <summary>
        /// Asserts that all the expected values are present in the actual values by ignoring
        /// the order of values.
        /// </summary>
        /// <param name="expectedValues">Expected subset of values</param>
        /// <param name="actualValues">Actual set of values</param>
        public static void Contains(
            IEnumerable<KeyValuePair<string, object>> expectedValues,
            IEnumerable<KeyValuePair<string, object>> actualValues)
        {
            if (expectedValues == null)
            {
                throw new ArgumentNullException(nameof(expectedValues));
            }

            if (actualValues == null)
            {
                throw new ArgumentNullException(nameof(actualValues));
            }

            var comparer = new SerilogLogValueComparer();

            foreach (var expectedPair in expectedValues)
            {
                if (!actualValues.Contains(expectedPair, comparer))
                {
                    throw new EqualException(
                        expected: GetString(expectedValues),
                        actual: GetString(actualValues));
                }
            }
        }

        /// <summary>
        /// Asserts that the given key and value are present in the log entry properties.
        /// </summary>
        /// <param name="key">The key of the item to be found.</param>
        /// <param name="value">The value of the item to be found.</param>
        /// <param name="logEntry">The log entry.</param>
        public static void Contains(
            string key,
            object value,
            SerilogLogEntry logEntry)
        {
            Contains(key, value, logEntry.Properties);
        }

        /// <summary>
        /// Asserts that all the expected values are present in the log entry properties by ignoring
        /// the order of values.
        /// </summary>
        /// <param name="expectedValues">Expected subset of values</param>
        /// <param name="logEntry">The log entry.</param>
        public static void Contains(
            IEnumerable<KeyValuePair<string, object>> expectedValues,
            SerilogLogEntry logEntry)
        {
            Contains(expectedValues, logEntry.Properties);
        }

        /// <summary>
        /// Asserts that the given key and value are present in the scope properties.
        /// </summary>
        /// <param name="key">The key of the item to be found.</param>
        /// <param name="value">The value of the item to be found.</param>
        /// <param name="scope">The scope.</param>
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
        public static void Contains(
            IEnumerable<KeyValuePair<string, object>> expectedValues,
            IScope scope)
        {
            Contains(expectedValues, scope.Properties);
        }

        private static string GetString(IEnumerable<KeyValuePair<string, object>> logValues)
        {
            return string.Join(",", logValues.Select(kvp => $"[{kvp.Key} {kvp.Value}]"));
        }

        private class SerilogLogValueComparer : IEqualityComparer<KeyValuePair<string, object>>
        {
            public bool Equals(KeyValuePair<string, object> x, KeyValuePair<string, object> y)
            {
                if (!string.Equals(x.Key, y.Key)) return false;

                bool result;
                if (x.Value is StructureValue xStructureValue && y.Value is StructureValue yStructureValue)
                {
                    var actualValues = yStructureValue.Properties;
                    var comparer = new SerilogPropertyComparer();
                    foreach (var expectedPair in xStructureValue.Properties)
                    {
                        if (!actualValues.Contains(expectedPair, comparer))
                        {
                            //throw new EqualException(
                            //    expected: GetString(expectedValues),
                            //    actual: GetString(actualValues));
                            throw new EqualException("blah", "blah");
                        }
                    }
                    result = Equals(xStructureValue.Properties, yStructureValue.Properties);
                }
                else
                {
                    result = x.Value.ToString() == y.Value.ToString();
                }

                return result;
            }

            public int GetHashCode(KeyValuePair<string, object> obj)
            {
                // We are never going to put this KeyValuePair in a hash table,
                // so this is ok.
                throw new NotImplementedException();
            }
        }

        private class SerilogPropertyComparer : IEqualityComparer<LogEventProperty>
        {
            public bool Equals(LogEventProperty x, LogEventProperty y)
            {
                if (!string.Equals(x.Name, y.Name)) return false;



                var result = x.Value.ToString() == y.Value.ToString();

                return result;
            }

            public int GetHashCode(LogEventProperty obj)
            {
                // We are never going to put this KeyValuePair in a hash table,
                // so this is ok.
                throw new NotImplementedException();
            }
        }

        class LogEventPropertyValueComparer : IEqualityComparer<LogEventPropertyValue>
        {
            public bool Equals(LogEventPropertyValue x, LogEventPropertyValue y)
            {
                if (x is ScalarValue scalarX && y is ScalarValue scalarY)
                {
                    return scalarX.Value.Equals(scalarY.Value);
                }

                if (x is SequenceValue sequenceX && y is SequenceValue sequenceY)
                {
                    return sequenceX.Elements
                        .SequenceEqual(sequenceY.Elements, this);
                }

                if (x is StructureValue || y is StructureValue)
                {
                    throw new NotImplementedException();
                }

                if (x is DictionaryValue || y is DictionaryValue)
                {
                    throw new NotImplementedException();
                }

                return false;
            }

            public int GetHashCode(LogEventPropertyValue obj) => 0;
        }
    }
}
