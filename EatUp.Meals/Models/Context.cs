using System.Linq.Expressions;
using System.Reflection.Metadata;
using System.Xml;
using Microsoft.EntityFrameworkCore;

namespace EatUp.Meals.Models
{
    public class Context : DbContext
    {
        public Context(DbContextOptions<Context> options) : base(options)
        {
        }

        public DbSet<Category> Categories { get; set; }

        public DbSet<Meal> Meals { get; set; }

        public DbSet<VendorProjection> VendorProjections { get; set; }

        public override int SaveChanges()
        {
            var entries = ChangeTracker.Entries()
                .Where(e => e.Entity is BaseEntity);

            foreach (var entry in entries)
            {
                BaseEntity entity = (BaseEntity)entry.Entity;
                var now = DateTime.UtcNow;
                entity.UpdatedAt = now;
                if (entry.State == EntityState.Added)
                {
                    entity.CreatedAt = now;
                }
                else if (entry.State == EntityState.Deleted)
                {
                    entity.DeletedAt = now;
                    entry.State = EntityState.Modified;
                }
            }

            return base.SaveChanges();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //Id is already Index, since it is a primary key in BaseEntity
            AddIndex<Meal>(modelBuilder, x => new { x.Title, x.VendorName, x.LastAvailablePickup }, false);
            AddIndex<Meal>(modelBuilder, x => x.VendorId, false);

            modelBuilder.Entity<Meal>()
                .HasMany(x => x.Categories)
                .WithMany(x => x.Meals)
                .UsingEntity(j => j.ToTable("MealsCategory"));

            modelBuilder.Entity<Meal>().HasQueryFilter(x => x.DeletedAt == null);

            base.OnModelCreating(modelBuilder);
        }

        public IQueryable<TEntity> GetQuery<TEntity>(bool tracking = false, params Expression<Func<TEntity, object>>[] includes) where TEntity : BaseEntity
        {
            IQueryable<TEntity> query = Set<TEntity>();

            if (!tracking)
            {
                query = query.AsNoTracking();
            }

            foreach (var include in includes)
            {
                query = query.Include(include);
            }

            return query;
        }

        private void AddIndex<TEntity>(ModelBuilder modelBuilder, Expression<Func<TEntity, object?>> column, bool isUnique = true) where TEntity : BaseEntity
        {
            var builder = modelBuilder.Entity<TEntity>()
                .HasIndex(column);
            if (isUnique)
                builder.IsUnique();

        }
    }
}
