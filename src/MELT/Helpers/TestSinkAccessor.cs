using System;
using System.Collections.Generic;
using System.Text;

namespace MELT
{
    public class TestSinkAccessor
    {
        private readonly ITestSink _testSink;

        public TestSinkAccessor(ITestSink testSink)
        {
            _testSink = testSink;
        }
    }
}
