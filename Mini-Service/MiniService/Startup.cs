using System.Reflection;
using MiniService.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Common.Service;
using MiniService.Application;
#if AddGrpc
using MiniService.Features.Todos;
#endif

namespace MiniService
{
    public class Startup
    {
        private const string ServiceName = nameof(MiniService);
        private readonly Assembly _currentAssembly = typeof(Startup).Assembly;

        public Startup(IConfiguration configuration) => Configuration = configuration;

        private IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddServiceCore();
            services.AddWebApi();
            services.AddMassTransitServices(Configuration, _currentAssembly);
#if AddGrpc
            services.AddGrpcServices(_currentAssembly);
#endif
            services.AddBackgroundWorker(Configuration);
            services.AddTelemetry(Configuration, ServiceName);
            services.AddApplication(Configuration);
            services.AddDatabase(Configuration);
            services.AddHealthChecksServices(Configuration);
        }

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
                endpoints.MapHealthChecksEndpoints();
            });
        }
    }
}
