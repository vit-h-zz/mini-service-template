using MiniService.Data.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace MiniService.Data.Persistence
{
    public partial class AppContext
    {
        public DbSet<TodoItem> TodoItems { get; set; } = default!;
    }
}
