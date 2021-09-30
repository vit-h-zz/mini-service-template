using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Sentry.AspNetCore;
using Sentry.AspNetCore.Grpc;
using Serilog;

namespace MiniService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseSerilog((context, services, config) => config
                    .ReadFrom.Configuration(context.Configuration)
                    //.ReadFrom.Services(services)
                    .WriteTo.Sentry())
                // This provides a simple way to "inject" our runtime configuration from a Kubernetes/OpenShift ConfigMap or Secret
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    config.AddJsonFile("/tmp/config/runtimesettings.json", optional: true, reloadOnChange: false);
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder
                        .ConfigureServices(c => _ = c.AddTransient<IStartupFilter, DevExceptionPageStartupFilter>())
                        .UseSentry(o => o
                            .AddGrpc()
                            .AddSentryOptions(x =>
                            {
                                x.DefaultTags.Add("Service", "MiniService");
                                x.SendDefaultPii = true;
                                x.AttachStacktrace = true;
                                //x.MinimumBreadcrumbLevel = LogLevel.Debug;
                                //x.MinimumEventLevel = LogLevel.Warning;
                            }))
                        .UseStartup<Startup>();
                });
    }
}
