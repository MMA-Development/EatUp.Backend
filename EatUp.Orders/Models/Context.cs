﻿using System.Linq.Expressions;
using System.Reflection.Metadata;
using System.Xml;
using Microsoft.EntityFrameworkCore;

namespace EatUp.Orders.Models
{
    public class Context : DbContext
    {
        public Context(DbContextOptions<Context> options) : base(options)
        {
        }

        public DbSet<Order> Orders { get; set; }

        public DbSet<VendorProjection> VendorProjections { get; set; }

        public DbSet<UserProjection> UserProjections { get; set; }

        public DbSet<MealProjection> MealProjections { get; set; }

        public override int SaveChanges()
        {
            var entries = ChangeTracker.Entries()
                .Where(e => e.Entity is BaseEntity && (e.State == EntityState.Added || e.State == EntityState.Modified || e.State == EntityState.Deleted));

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
            AddIndex<Order>(modelBuilder, x => x.UserId, false);
            AddIndex<Order>(modelBuilder, x => new
            {
                x.FoodPackageTitle,
                x.UserName,
                x.PaymentStatus,
                x.PaymentId,
                x.FoodPackageId
            });

            modelBuilder.Entity<Order>().HasQueryFilter(x => x.DeletedAt == null);
            modelBuilder.Entity<VendorProjection>().HasQueryFilter(x => x.DeletedAt == null);
            modelBuilder.Entity<UserProjection>().HasQueryFilter(x => x.DeletedAt == null);
            modelBuilder.Entity<MealProjection>().HasQueryFilter(x => x.DeletedAt == null);
            modelBuilder.Entity<Order>().Property(x => x.PaymentStatus).HasConversion<string>();

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
