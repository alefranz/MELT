using System;
using Microsoft.Extensions.Logging;

namespace MELT
{
    /// <summary>
    /// Options for configuring which log entries and scopes are captured by the test logger.
    /// </summary>
    public class TestLoggerOptions
    {
        internal Func<LogEntry, bool>? WriteEnabled { get; set; }
        internal Func<BeginScope, bool>? BeginEnabled { get; set; }

        /// <summary>
        /// Filters captured log entries and scopes to loggers whose name starts with the specified namespace prefix followed by a dot.
        /// </summary>
        /// <param name="namespacePrefix">The namespace prefix to match.</param>
        /// <returns>The current <see cref="TestLoggerOptions"/> instance for chaining.</returns>
        public TestLoggerOptions FilterByNamespace(string namespacePrefix)
        {
            AddWriteEnabledRule(x => x.LoggerName.StartsWith($"{namespacePrefix}."));
            AddBeginEnabledRule(x => x.LoggerName is not null && x.LoggerName.StartsWith($"{namespacePrefix}."));
            return this;
        }

        /// <summary>
        /// Filters captured log entries and scopes to loggers with the exact specified name.
        /// </summary>
        /// <param name="name">The logger name to match.</param>
        /// <returns>The current <see cref="TestLoggerOptions"/> instance for chaining.</returns>
        public TestLoggerOptions FilterByLoggerName(string name)
        {
            AddWriteEnabledRule(x => x.LoggerName == name);
            AddBeginEnabledRule(x => x.LoggerName == name);
            return this;
        }

        /// <summary>
        /// Filters captured log entries and scopes to loggers named after <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The type whose full name is used as the logger name filter.</typeparam>
        /// <returns>The current <see cref="TestLoggerOptions"/> instance for chaining.</returns>
        public TestLoggerOptions FilterByTypeName<T>() => FilterByLoggerName(typeof(T).FullName ?? typeof(T).Name);

        /// <summary>
        /// Filters captured log entries to those at or above the specified minimum log level.
        /// </summary>
        /// <param name="level">The minimum <see cref="LogLevel"/> to capture.</param>
        /// <returns>The current <see cref="TestLoggerOptions"/> instance for chaining.</returns>
        public TestLoggerOptions FilterByMinimumLevel(LogLevel level)
        {
            AddWriteEnabledRule(x => x.LogLevel >= level);
            return this;
        }

        internal static TestLoggerOptions Create(Action<TestLoggerOptions> configure)
        {
            if (configure == null) throw new ArgumentNullException(nameof(configure));

            var options = new TestLoggerOptions();
            configure(options);
            return options;
        }

        private void AddWriteEnabledRule(Func<LogEntry, bool> func)
        {
            if (WriteEnabled is null)
            {
                WriteEnabled = func;
            }
            else
            {
                var capturedWriteEnabled = WriteEnabled;
                WriteEnabled = x => capturedWriteEnabled(x) && func(x);
            }
        }

        private void AddBeginEnabledRule(Func<BeginScope, bool> func)
        {
            if (BeginEnabled is null)
            {
                BeginEnabled = func;
            }
            else
            {
                var capturedBeginEnabled = BeginEnabled;
                BeginEnabled = x => capturedBeginEnabled(x) && func(x);
            }
        }
    }
}
