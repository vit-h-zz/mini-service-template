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
#if AddGrpc
using Common.Service.Grpc;
#endif
using MiniService.Application;

namespace MiniService
{
    public class Startup
    {
        const string ServiceName = nameof(MiniService);

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // ReSharper disable once IdentifierTypo
        public void ConfigureServices(IServiceCollection s)
        {
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

            if (!Configuration.IsTrue("MassTransit:Disable"))
                s.AddMassTransitServices(Configuration, typeof(Startup).Assembly);

            s.AddApplication(Configuration);

            if (!Configuration.IsTrue("EF:Disable"))
                s.AddDbContextToDI(Configuration);

            if (!Configuration.IsTrue("HealthCheckz:Disable"))
                s.AddHealthChecksServices<MiniServiceDbContext>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
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
