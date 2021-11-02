using TemplateService.Data.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace TemplateService.Data.Persistence
{
    public partial class AppDbContext
    {
        public DbSet<TodoItem> TodoItems { get; set; } = default!;
    }
}
