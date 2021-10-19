using Common.Service;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace MiniService
{
    public static class DependencyInjection
    {
        private const string ReadyTag = "ready";
        private const string LiveTag = "live";

        public static void AddHealthChecksServices<TDbContext>(this IServiceCollection services)
            where TDbContext : DbContext
        {
            services.AddHealthChecks()
                .AddDbContextCheck<TDbContext>(tags: new[] {ReadyTag})
                .AddCheck("My custom always healthy check", () => HealthCheckResult.Healthy(), tags: new[] {ReadyTag})
                .AddCheck("Some leave check", () => HealthCheckResult.Healthy("Cache is healthy!"));
                //.AddCheck("Security Vault", () => /*call security vault*/);

            // Install more checks if needed "AspNetCore.HealthChecks...."
            //.AddCheck<MyCustomCheck>("My Custom Check")
            //.AddSqlServer(Configuration["ConnectionString"]) // Your database connection string
            //.AddDiskStorageHealthCheck(s => s.AddDrive("C:\\", 1024)) // 1024 MB (1 GB) free minimum
            //.AddProcessAllocatedMemoryHealthCheck(512) // 512 MB max allocated memory
            //.AddProcessHealthCheck("ProcessName", p => p.Length > 0) // check if process is running
            //.AddWindowsServiceHealthCheck("someservice", s => s.Status == ServiceControllerStatus.Running) // check if a windows service is running
            //.AddUrlGroup(new Uri("https://localhost:44318/weatherforecast"), "Example endpoint"); // should return status code 200

            services
                .AddHealthChecksUI()
                .AddInMemoryStorage();
        }

        public static void UseServiceHealthChecks(this IApplicationBuilder app)
        {
            app.UseHealthChecks("/health");
            app.UseHealthChecksUI(o =>
            {
                o.UIPath = "/health-ui";
                o.ApiPath = "/health-ui-api";
            });
        }

        public static void MapHealthChecksEndpoints(this IEndpointRouteBuilder routeBuilder)
        {
            routeBuilder.MapHealthChecks("/healthz/ready", new HealthCheckOptions { Predicate = check => check.Tags.Contains(ReadyTag) });
            routeBuilder.MapHealthChecks("/healthz/live", new HealthCheckOptions { Predicate = check => check.Tags.Contains(LiveTag) });
            routeBuilder.MapHealthChecks("/healthz", new HealthCheckOptions
            {
                Predicate = _ => true,
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
            });
        }

        public static IServiceCollection AddGrpcServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.RegisterClasses(searchPattern: "*Service", typeof(DependencyInjection).Assembly);

            return services;
        }

        public static IServiceCollection AddBackgroundWorker(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<BackgroundWorkerOptions>(c => c.MaxStoppingDelayMs = 100_000);
            services.AddSingleton<BackgroundWorker>();
            services.AddHostedService(sp => sp.GetRequiredService<BackgroundWorker>());

            return services;
        }

        public static bool IsTrue(this IConfiguration configuration, string key) => configuration.GetValue<bool?>(key) == true;
    }
}
