using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;

namespace MELT.Xunit.Tests
{
    public static class TestLoggerBuilder
    {
        public static ILoggerFactory Create(Action<ILoggingBuilder> configure)
        {
            return new ServiceCollection()
                .AddLogging(configure)
                .BuildServiceProvider()
                .GetRequiredService<ILoggerFactory>();
        }
    }
}
