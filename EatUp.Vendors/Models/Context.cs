using System.Linq.Expressions;
using System.Reflection.Metadata;
using System.Xml;
using Microsoft.EntityFrameworkCore;

namespace EatUp.Vendors.Models
{
    public class Context : DbContext
    {
        public Context(DbContextOptions<Context> options) : base(options)
        {
        }

        public DbSet<Vendor> Vendors { get; set; }


        public override int SaveChanges()
        {
            var entries = ChangeTracker.Entries()
                .Where(e => e.Entity is BaseEntity);

            foreach (var entry in entries)
            {
                BaseEntity entity = (BaseEntity)entry.Entity;

                entity.UpdatedAt = DateTime.UtcNow;
                if (entry.State == EntityState.Added)
                {
                    entity.CreatedAt = DateTime.UtcNow;
                }
                else if (entry.State == EntityState.Deleted)
                {
                    entity.DeletedAt = DateTime.UtcNow;
                    entry.State = EntityState.Modified;
                }
            }

            return base.SaveChanges();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //Id is already Index, since it is a primary key in BaseEntity
            AddIndex<Vendor>(modelBuilder, x => x.Username);
            AddIndex<Vendor>(modelBuilder, x => x.StripeAccountId);
            AddIndex<Vendor>(modelBuilder, x => x.Name, false);

            modelBuilder.Entity<Vendor>().HasQueryFilter(x => x.DeletedAt == null);
            modelBuilder.Entity<RefreshTokenInformation>().HasQueryFilter(x => x.DeletedAt == null || x.ExpirationDate < DateTime.Now);

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
