using MiniService.Data.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MiniService.Data
{
    public static class DependencyInjection
    {
        /// <summary>
        /// Adds Database with HealthCheck
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration">Containing <see cref="DatabaseOptions"/></param>
        /// <returns></returns>
        public static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
        {
            var options = configuration.GetSection(DatabaseOptions.SectionName).Get<DatabaseOptions>();

            if (!options.Enable)
                return services;

            if (options.HealthChecks?.Enable ?? false)
                services.AddHealthChecks().AddDbContextCheck<AppDbContext>(tags: options.HealthChecks.Tags);

            if (options.UseInMemory)
                return services.AddDbContext<AppDbContext>(o =>
                {
                    o.UseInMemoryDatabase("MiniServiceDb");

                    if (!options.DetailedErrors)
                        return;

                    o.EnableDetailedErrors();
                    o.EnableSensitiveDataLogging();
                });

            // To enable JsonB PostgreSQL columns
            // NpgsqlConnection.GlobalTypeMapper.UseJsonNet();

            return services.AddDbContext<AppDbContext>(o =>
            {
                o.UseNpgsql(options.ConnectionString, b =>
                    {
                        b.SetPostgresVersion(options.Version);
                        //b.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName);
                    });

                if (!options.DetailedErrors)
                    return;

                o.EnableDetailedErrors();
                o.EnableSensitiveDataLogging();
            });
        }
    }
}
