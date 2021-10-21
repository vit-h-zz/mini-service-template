using System;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace MiniService.Data.Persistence
{
    /// <summary>
    /// Helper to create DbContext when creating/applying migrations
    /// since 'MiniService.Data.csproj' is 'Library' and not an executable.
    /// </summary>
    // more info https://docs.microsoft.com/en-us/ef/core/cli/dbcontext-creation
    public class DbContextDesignFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        private const string ConnectionStringPath = "Database:ConnectionString";
        private const string RelativePathToConfig = @"..\MiniService\appsettings.json";

        public AppDbContext CreateDbContext(string[] args)
        {
            // dotnet ef database update -- FirstAppArg "second argument" ThirdAppArg
            // Console.WriteLine($"args: {string.Join("; ", args)}");

            var path = Path.GetFullPath($@"{Directory.GetCurrentDirectory()}\{RelativePathToConfig}");

            Console.WriteLine($"Assessing: {path}");

            var configuration = new ConfigurationBuilder().AddJsonFile(path).Build();
            var connectionString = configuration[ConnectionStringPath];

            Console.WriteLine($"{ConnectionStringPath}: '{connectionString}'");

            var options = new DbContextOptionsBuilder<AppDbContext>();
            options.UseNpgsql(connectionString);
            //options.UseSqlServer(connectionString);

            return new AppDbContext(options.Options);
        }
    }
}
