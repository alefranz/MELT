using Microsoft.Extensions.Logging;
using System;

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
    }
}
