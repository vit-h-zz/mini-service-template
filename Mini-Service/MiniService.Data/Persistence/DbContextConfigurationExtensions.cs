using System;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace MiniService.Data.Persistence
{
    public static class DbContextConfigurationExtensions
    {
        public static Version GetPostgresVersion()
        {
            return new Version(9, 6);
        }

        public static DbContextOptionsBuilder ConfigureForCTT(this DbContextOptionsBuilder options, string connectionString, bool isDevelopment = false)
        {
            var o = options
                .UseNpgsqlForCTT(connectionString);
            //.UseLazyLoadingProxies();

            return !isDevelopment
                ? o
                : o.AddDevelopmentOptions();
        }

        public static DbContextOptionsBuilder UseNpgsqlForCTT(this DbContextOptionsBuilder options, string connectionString)
        {
            NpgsqlConnection.GlobalTypeMapper.UseJsonNet();

            return options
                .UseNpgsql(connectionString, o => o.SetPostgresVersion(GetPostgresVersion()));
        }

        public static DbContextOptionsBuilder AddDevelopmentOptions(this DbContextOptionsBuilder options)
        {
            return options
                .EnableDetailedErrors()
                .EnableSensitiveDataLogging();
        }
    }
}
