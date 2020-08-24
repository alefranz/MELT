using System;
using Microsoft.Extensions.Logging;

namespace SampleLibrary
{
    public class Sample
    {
        private readonly ILogger<Sample> _logger;

        public Sample(ILogger<Sample> logger)
        {
            _logger = logger;
        }

        public void DoSomething()
        {
            _logger.LogInformation("The answer is {number}", 42);
        }

        public void DoExceptional()
        {
            var exception = new ArgumentNullException("foo");
            _logger.LogError(exception, "There was a {error}", "problem");
        }
    }
}
