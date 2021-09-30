using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace MiniService.Data.Persistence
{
    public class DbContextDesignFactory : IDesignTimeDbContextFactory<MiniServiceDbContext>
    {
        public MiniServiceDbContext CreateDbContext(string[] args)
        {
            var options = new DbContextOptionsBuilder<MiniServiceDbContext>();
            options.ConfigureForCTT("localhost");
            return new MiniServiceDbContext(options.Options);
        }
    }
}
