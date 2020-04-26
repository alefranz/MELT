using Microsoft.Extensions.Logging;
using System;

namespace MELT
{
    public class TestLoggerOptions
    {
        internal Func<WriteContext, bool>? WriteEnabled { get; set; }
        internal Func<BeginScopeContext, bool>? BeginEnabled { get; set; }

        public bool UseScopeFromProperties { get; set; }

        public TestLoggerOptions FilterByNamespace(string namespacePrefix)
        {
            AddWriteEnabledRule(x => x.LoggerName.StartsWith($"{namespacePrefix}."));
            AddBeginEnabledRule(x => x.LoggerName.StartsWith($"{namespacePrefix}."));
            return this;
        }

        public TestLoggerOptions FilterByLoggerName(string name)
        {
            AddWriteEnabledRule(x => x.LoggerName == name);
            AddBeginEnabledRule(x => x.LoggerName == name);
            return this;
        }

        public TestLoggerOptions FilterByTypeName<T>() => FilterByLoggerName(typeof(T).FullName);

        public TestLoggerOptions FilterByMinimumLevel(LogLevel level)
        {
            AddWriteEnabledRule(x => x.LogLevel >= level);
            return this;
        }

        private void AddWriteEnabledRule(Func<WriteContext, bool> func)
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

        private void AddBeginEnabledRule(Func<BeginScopeContext, bool> func)
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
