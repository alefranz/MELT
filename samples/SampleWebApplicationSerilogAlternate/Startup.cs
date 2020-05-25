using MELT;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SampleWebApplicationSerilogAlternate
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddLogging(loggingBuilder => loggingBuilder.ClearProviders());
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/", async context =>
                {
                    var logger = context.RequestServices.GetRequiredService<ILogger<Startup>>();
                    await GetAsync(context, logger);
                });

                endpoints.MapGet("/nestedScope", async context =>
                {
                    var logger = context.RequestServices.GetRequiredService<ILogger<Startup>>();
                    using (logger.BeginScope("A top level scope"))
                    {
                        await GetAsync(context, logger);
                    }
                });

                endpoints.MapGet("/dictionaryScope", context =>
                {
                    var logger = context.RequestServices.GetRequiredService<ILogger<Startup>>();
                    using (logger.BeginScope(new Dictionary<string, object>() { ["foo"] = "bar", ["answer"] = 42 }))
                    {
                        logger.LogInformation("foo");
                        context.Response.StatusCode = StatusCodes.Status204NoContent;
                        return Task.CompletedTask;
                    }
                });

                endpoints.MapGet("/destructure", context =>
                {
                    var logger = context.RequestServices.GetRequiredService<ILogger<Startup>>();
                    var thing = new { foo = "bar", answer = 42 };
                    logger.LogInformation("This {@thing} has been destructured.", thing);
                    context.Response.StatusCode = StatusCodes.Status204NoContent;
                    return Task.CompletedTask;
                });

                endpoints.MapGet("/array", context =>
                {
                    var logger = context.RequestServices.GetRequiredService<ILogger<Startup>>();
                    var array = new[] { 1, 2 };
                    logger.LogInformation("This {array} is an array.", array);
                    context.Response.StatusCode = StatusCodes.Status204NoContent;
                    return Task.CompletedTask;
                });
            });
        }

        private static async Task GetAsync(HttpContext context, ILogger<Startup> logger)
        {
            using (logger.BeginScope("I'm in the {name} scope", "GET"))
            {
                logger.LogInformation("Hello {place}!", "World");
                context.Response.ContentType = "text/plain";
                await context.Response.WriteAsync("Hello World!");
            }
        }
    }
}
