using Microsoft.Extensions.Logging;
using System;

namespace MELT
{
    public static class MELTBuilder
    {
        public static TestLoggerFactory CreateLoggerFactory() => new TestLoggerFactory(new TestSink(), true);

        public static TestSink CreateTestSink(Action<TestSinkOptions> configure)
        {
            var options = new TestSinkOptions();
            configure(options);
            return new TestSink(options.WriteEnabled, options.BeginEnabled);
        }
    }
}
