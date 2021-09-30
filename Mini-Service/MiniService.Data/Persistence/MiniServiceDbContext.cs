using MiniService.Data.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace MiniService.Data.Persistence
{
    public partial class MiniServiceDbContext
    {
        public DbSet<TodoItem> TodoItems { get; set; } = default!;
    }
}
