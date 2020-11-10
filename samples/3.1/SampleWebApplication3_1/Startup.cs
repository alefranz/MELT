using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace SampleWebApplication3_1
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
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
            });
        }
    }
}
