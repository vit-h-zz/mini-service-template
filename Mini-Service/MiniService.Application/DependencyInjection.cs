using Common.Application;
using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MiniService.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);
            services.AddMediatrPipeline(configuration, typeof(DependencyInjection).Assembly);

            if (!configuration.IsTrue("MassTransit:Disable"))
                services.AddMassTransitClient();

            services.AddNodaClock();

            return services;
        }

        public static bool IsTrue(this IConfiguration configuration, string key) => configuration.GetValue<bool?>(key) == true;
    }
}
