using MiniService.Data.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MiniService.Data.Persistence.Configurations
{
    public class TodoItemConfiguration : IEntityTypeConfiguration<TodoItem>
    {
        public void Configure(EntityTypeBuilder<TodoItem> b)
        {
            b.Property(t => t.Title).HasMaxLength(200).IsRequired();
            b.Property(t => t.Updated).IsConcurrencyToken();
        }
    }
}
