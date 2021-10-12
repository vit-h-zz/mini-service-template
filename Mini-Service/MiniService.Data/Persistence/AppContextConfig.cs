using System;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using MiniService.Data.Domain.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace MiniService.Data.Persistence
{
    public sealed partial class AppContext : DbContext
    {
        public const int DecimalPrecision = 28;
        public const int DecimalScale = 15;
        private readonly string _decimalColumn = $"decimal({DecimalPrecision}, {DecimalScale})";

        public AppContext(DbContextOptions<AppContext> options) : base(options)
        {
            ChangeTracker.CascadeDeleteTiming = CascadeTiming.OnSaveChanges;
            ChangeTracker.DeleteOrphansTiming = CascadeTiming.OnSaveChanges;
        }

        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken ct = default)
        {
            HandleAdded();
            HandleModified();
            HandleValidatable();
            HandleVersioned();

            return base.SaveChangesAsync(acceptAllChangesOnSuccess, ct);
        }

        public override int SaveChanges(bool acceptAllChangesOnSuccess) => throw new NotSupportedException("Use async version instead!");

        private void HandleAdded()
        {
            var added = ChangeTracker.Entries<IAuditableEntity>().Where(e => e.State == EntityState.Added);

            foreach (var entry in added)
            {
                entry.Property(x => x.Created).CurrentValue = DateTime.UtcNow;
                entry.Property(x => x.Created).IsModified = true;

                entry.Property(x => x.Updated).CurrentValue = DateTime.UtcNow;
                entry.Property(x => x.Updated).IsModified = true;
            }
        }

        private void HandleModified()
        {
            var modified = ChangeTracker.Entries<IAuditableEntity>().Where(e => e.State == EntityState.Modified);

            foreach (var entry in modified)
            {
                entry.Property(x => x.Updated).CurrentValue = DateTime.UtcNow;
                entry.Property(x => x.Updated).IsModified = true;

                entry.Property(x => x.Created).CurrentValue = entry.Property(x => x.Created).OriginalValue;
                entry.Property(x => x.Created).IsModified = false;
            }
        }

        private void HandleValidatable()
        {
            var validatableModels = ChangeTracker.Entries<IValidatableModel>();

            foreach (var model in validatableModels)
                model.Entity.ValidateAndThrow();
        }

        private void HandleVersioned()
        {
            foreach (var versionedModel in ChangeTracker.Entries<IVersionedEntity>())
            {
                var versionProp = versionedModel.Property(o => o.Version);

                if (versionedModel.State == EntityState.Added)
                {
                    versionProp.CurrentValue = 1;
                }
                else if (versionedModel.State == EntityState.Modified)
                {
                    versionProp.CurrentValue = versionProp.OriginalValue + 1;
                    versionProp.IsModified = true;
                }
            }
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            base.OnModelCreating(builder);

            #region Set Decimal Precision

            var decimals = builder.Model.GetEntityTypes()
                .SelectMany(t => t.GetProperties())
                .Where(p => p.ClrType == typeof(decimal));

            foreach (var property in decimals)
                property.SetColumnType(_decimalColumn);

            #endregion
        }

        public override Task<int> SaveChangesAsync(CancellationToken ct = new())
        {
            if (Database.IsInMemory())
            {
                ct.ThrowIfCancellationRequested();
            }

            return base.SaveChangesAsync(ct);
        }
    }
}
