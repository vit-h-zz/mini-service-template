using System.Reflection;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using VH.MiniService.Common.Service.Grpc;

namespace TemplateService
{
    public static class DependencyInjection
    {
        private const string ReadyTag = "ready";
        private const string LiveTag = "live";
        private static AppHealthChecksOptions? _healthOptions = null!;

        public static IServiceCollection AddHealthChecksServices(this IServiceCollection services, IConfiguration configuration)
        {
            _healthOptions = configuration.GetSection(AppHealthChecksOptions.SectionName).Get<AppHealthChecksOptions>();

            if (_healthOptions == null || !_healthOptions.Enable)
                return services;

            services.AddHealthChecks()
                .AddCheck("My custom always healthy check", () => HealthCheckResult.Healthy(), tags: new[] { ReadyTag });

            // Install more checks if needed "AspNetCore.HealthChecks...."
            //.AddCheck<MyCustomCheck>("My Custom Check")
            //.AddSqlServer(Configuration["ConnectionString"]) // Your database connection string
            //.AddDiskStorageHealthCheck(s => s.AddDrive("C:\\", 1024)) // 1024 MB (1 GB) free minimum
            //.AddProcessAllocatedMemoryHealthCheck(512) // 512 MB max allocated memory
            //.AddProcessHealthCheck("ProcessName", p => p.Length > 0) // check if process is running
            //.AddWindowsServiceHealthCheck("someservice", s => s.Status == ServiceControllerStatus.Running) // check if a windows service is running
            //.AddUrlGroup(new Uri("https://localhost:44318/weatherforecast"), "Example endpoint"); // should return status code 200

            if (!_healthOptions.EnableUi)
                return services;

            services
                .AddHealthChecksUI()
                .AddInMemoryStorage();

            return services;
        }

        public static IApplicationBuilder UseServiceHealthChecks(this IApplicationBuilder app)
        {
            if (_healthOptions == null || !_healthOptions.Enable || !_healthOptions.EnableUi)
                return app;

            return app
                .UseHealthChecks("/health")
                .UseHealthChecksUI(o =>
                {
                    o.UIPath = "/health-ui";
                    o.ApiPath = "/health-ui-api";
                });
        }

        public static void MapHealthChecksEndpoints(this IEndpointRouteBuilder routeBuilder)
        {
            if (_healthOptions == null || !_healthOptions.Enable)
                return;

            routeBuilder.MapHealthChecks("/healthz/ready", new HealthCheckOptions { Predicate = check => check.Tags.Contains(ReadyTag) });
            routeBuilder.MapHealthChecks("/healthz/live", new HealthCheckOptions { Predicate = check => check.Tags.Contains(LiveTag) });
            routeBuilder.MapHealthChecks("/healthz", new HealthCheckOptions
            {
                Predicate = _ => true,
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
            });
        }

        public static IServiceCollection AddGrpcServices(this IServiceCollection services, params Assembly[] assemblies)
        {
            //services.RegisterClasses(searchPattern: "*Service", assemblies);
            services.Scan(scan => scan
                .FromAssemblies(assemblies)
                .AddClasses(o => o.Where(x => x.Name.EndsWith("Service") && (x.BaseType?.Namespace?.StartsWith("Grpc.") ?? false)))
                .AsSelf()
                .WithScopedLifetime());

            services.AddGrpc(o =>
            {
                o.Interceptors.Add<ClientCancelledInterceptor>();
                o.Interceptors.Add<UserContextInterceptor>();
            });

            return services;
        }

        public static IServiceCollection AddBackgroundWorker(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<BackgroundWorkerOptions>(c => c.MaxStoppingDelayMs = 100_000);
            services.AddSingleton<BackgroundWorker>();
            services.AddHostedService(sp => sp.GetRequiredService<BackgroundWorker>());

            return services;
        }
    }
}
