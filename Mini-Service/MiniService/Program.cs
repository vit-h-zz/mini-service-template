using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
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
                    )

                // This provides a simple way to "inject" our runtime configuration from a Kubernetes/OpenShift ConfigMap or Secret
                .ConfigureAppConfiguration((hostingContext, config) => config.AddJsonFile("/config/runtimesettings.json", optional: true, reloadOnChange: false))

                .ConfigureWebHostDefaults(webBuilder => webBuilder.UseStartup<Startup>());
    }
}
