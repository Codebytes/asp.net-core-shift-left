using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using AspNetCoreShiftLeft.Web.Health;
using HealthChecks.System;

namespace AspNetCoreShiftLeft.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHealthChecks()
                    .AddCheck("Self", () => HealthCheckResult.Healthy())
                    .AddMemoryHealthCheck("Memory")
                    .AddDiskStorageHealthCheck((DiskStorageOptions diskStorageOptions) =>
                    {
                        diskStorageOptions.AddDrive(@"c:\", 5000);
                    }, "C Drive", HealthStatus.Degraded)
                    .AddApplicationInsightsPublisher();
            services.AddHealthChecksUI();
            
            services.AddApplicationInsightsTelemetry(Configuration);
            services.AddRazorPages();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHealthChecks("/hc", new HealthCheckOptions
                {
                    Predicate = registration => true,
                    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
                }); ;
                endpoints.MapHealthChecksUI();
                endpoints.MapRazorPages();
            });
        }
    }
}