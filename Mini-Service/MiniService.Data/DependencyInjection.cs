using MiniService.Data.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MiniService.Data
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddDbContextToDI(this IServiceCollection services, IConfiguration configuration)
        {
            if (configuration.GetValue<bool>("UseInMemoryDatabase"))
            {
                services.AddDbContext<MiniServiceDbContext>(options =>
                    options.UseInMemoryDatabase("MiniServiceDb"));
            }
            else
            {
                services.AddDbContext<MiniServiceDbContext>(options =>
                    options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")
                        , b => b.MigrationsAssembly(typeof(MiniServiceDbContext).Assembly.FullName)
                    ));
            }

            return services;
        }
    }
}
