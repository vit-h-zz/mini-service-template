using System;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace MiniService.Data.Persistence
{
    // more info https://docs.microsoft.com/en-us/ef/core/cli/dbcontext-creation
    public class DbContextDesignFactory : IDesignTimeDbContextFactory<AppContext>
    {
        private const string connectionStringPath = "ConnectionStrings:DefaultConnection";
        private const string relativePathToConfig = @"..\MiniService\appsettings.json";

        public AppContext CreateDbContext(string[] args)
        {
            // dotnet ef database update -- FirstAppArg "second argument" ThirdAppArg
            // Console.WriteLine($"args: {string.Join("; ", args)}");

            var path = Path.GetFullPath($@"{Directory.GetCurrentDirectory()}\{relativePathToConfig}");

            Console.WriteLine($"Assessing: {path}");

            var configuration = new ConfigurationBuilder().AddJsonFile(path).Build();
            var connectionString = configuration[connectionStringPath];

            Console.WriteLine($"{connectionStringPath}: '{connectionString}'");

            var options = new DbContextOptionsBuilder<AppContext>();
            options.UseNpgsql(connectionString);
            //options.UseSqlServer(connectionString);

            return new AppContext(options.Options);
        }
    }
}
