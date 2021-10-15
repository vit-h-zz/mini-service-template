using MiniService.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MiniService.Data.Persistence;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using System.Text.RegularExpressions;
using Common.Service;
using Common.Service.Options;
#if AddGrpc
using Common.Service.Grpc;
using MiniService.Features.Todos;
#endif
using MiniService.Application;

namespace MiniService
{
    public class Startup
    {
        private const string ServiceName = nameof(MiniService);

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection s)
        {
            //s.AddHostedService<BackgroundWorker>();
            //s.AddSingleton<IBackgroundTaskQueue<Func<CancellationToken, ValueTask>>>(ctx =>
            //{
            //    if (!int.TryParse(Configuration["QueueCapacity"], out var queueCapacity))
            //        queueCapacity = 100;
            //    return new BackgroundTaskQueue<Func<CancellationToken, ValueTask>>(queueCapacity);
            //});

            s.AddServiceCore();

            s.AddTelemetry(Configuration, ServiceName);

#if AddGrpc
            s.AddGrpc(o =>
            {
                o.Interceptors.Add<ClientCancelledInterceptor>();
            });

            s.AddGrpcServices(Configuration);
#endif
            s.AddSwaggerGen(o => o.DescribeAllParametersInCamelCase());
            s.AddControllers(o => o.Conventions.Add(new RouteTokenTransformerConvention(new KebabCaseRouteTransformer())))
                .AddControllersAsServices();

            s.AddMassTransitServices(Configuration.GetOptions<MassTransitOptions>(), typeof(Startup).Assembly);

            s.AddApplication(Configuration);

            if (!Configuration.IsTrue("EF:Disable"))
                s.AddDbContextToDI(Configuration);

            if (!Configuration.IsTrue("HealthCheckz:Disable"))
                s.AddHealthChecksServices<AppDbContext>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger(c => c.RouteTemplate = "docs/{documentName}/spec.json");
                app.UseSwaggerUI(c =>
                {
                    c.RoutePrefix = "docs";
                    c.SwaggerEndpoint("/docs/v1/spec.json", ServiceName);
                });
            }

            if (!Configuration.IsTrue("HealthCheckz:Disable"))
                app.UseServiceHealthChecks();

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
#if AddGrpc
                endpoints.MapGrpcService<TodosService>();
#endif
                endpoints.MapControllers();

                if (!Configuration.IsTrue("HealthCheckz:Disable"))
                    endpoints.MapHealthChecksEndpoints();
            });
        }
    }

    public class KebabCaseRouteTransformer : IOutboundParameterTransformer
    {
        public string? TransformOutbound(object? value) => value == null ? null : Regex.Replace(value.ToString()!, "([a-z])([A-Z])", "$1-$2").ToLower();
    }
}
