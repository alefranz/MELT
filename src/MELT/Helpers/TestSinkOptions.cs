using System;
using Microsoft.Extensions.Logging;

namespace MELT
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public class TestLoggerOptions
    {
#pragma warning disable CS0612 // Type or member is obsolete
        internal Func<WriteContext, bool>? WriteEnabled { get; set; }
        internal Func<BeginScopeContext, bool>? BeginEnabled { get; set; }
#pragma warning restore CS0612 // Type or member is obsolete

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

#pragma warning disable CS0612 // Type or member is obsolete
        private void AddWriteEnabledRule(Func<WriteContext, bool> func)
#pragma warning restore CS0612 // Type or member is obsolete
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

#pragma warning disable CS0612 // Type or member is obsolete
        private void AddBeginEnabledRule(Func<BeginScopeContext, bool> func)
#pragma warning restore CS0612 // Type or member is obsolete
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
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}
