using MiniService.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MiniService.Data.Persistence;
using Common.Service;
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

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddServiceCore();

            services.AddWebApi();

            services.AddMassTransitServices(Configuration, typeof(Startup).Assembly);

#if AddGrpc
            services.AddGrpc(o =>
            {
                o.Interceptors.Add<ClientCancelledInterceptor>();
                o.Interceptors.Add<UserContextInterceptor>();
            });

            services.AddGrpcServices(Configuration);
#endif

            services.AddBackgroundWorker(Configuration);

            services.AddTelemetry(Configuration, ServiceName);

            services.AddApplication(Configuration);

            if (!Configuration.IsTrue("EF:Disable"))
                services.AddDbContextToDI(Configuration);

            if (!Configuration.IsTrue("HealthCheckz:Disable"))
                services.AddHealthChecksServices<AppDbContext>();
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
}
