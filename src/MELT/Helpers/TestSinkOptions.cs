using Microsoft.Extensions.Logging;
using System;

namespace MELT
{
    public class TestSinkOptions
    {
        internal Func<WriteContext, bool> WriteEnabled { get; private set; } = x => true;
        internal Func<BeginScopeContext, bool> BeginEnabled { get; private set; } = x => true;

        public void FilterByNamespace(string namespacePrefix)
        {
            AddWriteEnabledRule(x => x.LoggerName.StartsWith($"{namespacePrefix}."));
            AddBeginEnabledRule(x => x.LoggerName.StartsWith($"{namespacePrefix}."));
        }

        public void FilterByLoggerName(string name)
        {
            AddWriteEnabledRule(x => x.LoggerName == name);
            AddBeginEnabledRule(x => x.LoggerName == name);
        }

        public void SetMinimumLevel(LogLevel level)
        {
            AddWriteEnabledRule(x => x.LogLevel >= level);
        }

        private void AddWriteEnabledRule(Func<WriteContext, bool> func)
        {
            var capturedWriteEnabled = WriteEnabled;
            WriteEnabled = x => capturedWriteEnabled(x) && func(x);
        }

        private void AddBeginEnabledRule(Func<BeginScopeContext, bool> func)
        {
            var capturedBeginEnabled = BeginEnabled;
            BeginEnabled = x => capturedBeginEnabled(x) && func(x);
        }
    }
}
