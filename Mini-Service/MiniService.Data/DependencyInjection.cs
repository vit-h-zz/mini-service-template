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
                services.AddDbContext<AppContext>(options =>
                    options.UseInMemoryDatabase("MiniServiceDb"));
            }
            else
            {
                services.AddDbContext<AppContext>(options =>
                    options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")
                        //, b => b.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName)
                    ));
            }

            return services;
        }
    }
}
