using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Threading.Tasks;
using MELT.Xunit;
using Xunit;
using Xunit.Abstractions;
using Microsoft.AspNetCore.Http;
using System.Net;

namespace SampleWebApplication.Tests
{
    public class ShowLogsInTestOutputExample : IClassFixture<WebApplicationFactory<Startup>>
    {
        private readonly WebApplicationFactory<Startup> _factory;

        public ShowLogsInTestOutputExample(WebApplicationFactory<Startup> factory, ITestOutputHelper _output)
        {
            _factory = factory.WithWebHostBuilder(builder => builder
                .ConfigureLogging(logging => logging.AddXunit(_output)));
        }

        [Fact]
        public async Task ThisTestShowsLogsInTestOutput()
        {
            // Act
            var result = await _factory.CreateDefaultClient().GetAsync("/");

            // Assert
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
        }
    }
}
