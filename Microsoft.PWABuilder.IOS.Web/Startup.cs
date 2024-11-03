using Microsoft.ApplicationInsights.AspNetCore.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Microsoft.PWABuilder.IOS.Web.Models;
using Microsoft.PWABuilder.IOS.Web.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using PWABuilder.IOS.Services;
using PWABuilder.IOS.Services.Models;

namespace Microsoft.PWABuilder.IOS.Web
{
    public class Startup
    {
        private readonly string AllowedOriginsPolicyName = "allowedOrigins";

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var appSettings = Configuration.GetSection("AppSettings");
            var aiOptions = setUpAppInsights(appSettings);
            services.Configure<AppSettings>(Configuration.GetSection("AppSettings"));
            services.AddCors(options =>
            {
                options.AddPolicy(name: AllowedOriginsPolicyName, builder => builder
                    .SetIsOriginAllowed(CheckAllowedOriginCors)
                    .AllowAnyHeader()
                    .AllowAnyMethod());
            });
            services.AddBuilderServices();
            services.AddTransient<TempDirectory>();
            services.AddTransient<AnalyticsService>();
            services.AddSingleton(c => c.GetService<IOptions<AppSettings>>()!.Value);
            services.AddHttpClient();
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Microsoft.PWABuilder.IOS.Web", Version = "v1" });
            });
            services.AddApplicationInsightsTelemetry(aiOptions);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Microsoft.PWABuilder.IOS.Web v1"));
            }

            app.UseDeveloperExceptionPage();
            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseStaticFiles();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        private bool CheckAllowedOriginCors(string origin)
        {
            var allowedOrigins = new[]
            {
                "https://www.pwabuilder.com",
                "https://pwabuilder.com",
                "https://preview.pwabuilder.com",
                "https://localhost:3333",
                "https://localhost:3000",
                "http://localhost:3333",
                "http://localhost:3000",
                "https://localhost:8000",
                "http://localhost:8000",
                "https://nice-field-047c1420f.azurestaticapps.net"
            };
            return allowedOrigins.Any(o => origin.Contains(o, StringComparison.OrdinalIgnoreCase));
        }

        static ApplicationInsightsServiceOptions setUpAppInsights(IConfigurationSection appSettings)
        {
            var connectionString = appSettings["ApplicationInsightsConnectionString"];
            var aiOptions = new ApplicationInsightsServiceOptions();
            aiOptions.EnableRequestTrackingTelemetryModule = false;
            aiOptions.EnableDependencyTrackingTelemetryModule = true;
            aiOptions.EnableHeartbeat = false;
            aiOptions.EnableAzureInstanceMetadataTelemetryModule = false;
            aiOptions.EnableActiveTelemetryConfigurationSetup = false;
            aiOptions.EnableAdaptiveSampling = false;
            aiOptions.EnableAppServicesHeartbeatTelemetryModule = false;
            aiOptions.EnableAuthenticationTrackingJavaScript = false;
            aiOptions.ConnectionString = connectionString;
            return aiOptions;
        }
    }
}
