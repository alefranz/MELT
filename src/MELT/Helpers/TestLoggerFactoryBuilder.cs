using Microsoft.Extensions.Logging;

namespace MELT
{
    public static class TestLoggerFactoryBuilder
    {
        public static TestLoggerFactory CreateLoggerFactory() => new TestLoggerFactory(new TestSink(), true);
    }
}
