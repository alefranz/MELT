using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace SampleWebApplication
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices()
        {
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/", async context =>
                {
                    var logger = context.RequestServices.GetRequiredService<ILogger<Startup>>();
                    using (logger.BeginScope("I'm in the {name} scope", "GET"))
                    {
                        if (context.Request.Query.ContainsKey("multipleValues"))
                        {
                            logger.LogInformation("Hello {place} and {place}!", "World", "Universe");
                        }
                        else
                        {
                            logger.LogInformation("Hello {place}!", "World");
                        }
                        context.Response.ContentType = "text/plain";
                        await context.Response.WriteAsync("Hello World!");
                    }
                });

                endpoints.MapGet("/incorrectlyDisposedScopes", async context =>
                {
                    var logger = context.RequestServices.GetRequiredService<ILogger<Startup>>();

                    using (var a = logger.BeginScope("A"))
                    {
                        var b = logger.BeginScope("B");
                        var c = logger.BeginScope("C");

                        logger.LogInformation("Hello {place}!", "World");

                        b?.Dispose();

                        logger.LogInformation("Hello {place}!", "World");

                        c?.Dispose();

                        logger.LogInformation("Hello {place}!", "World");

                        context.Response.ContentType = "text/plain";
                        await context.Response.WriteAsync("Hello World!");
                    }

                    logger.LogInformation("Hello {place}!", "World");
                });
            });
        }
    }
}
