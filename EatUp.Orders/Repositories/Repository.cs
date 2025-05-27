using System.Linq.Expressions;
using EatUp.Orders.Extensions;
using EatUp.Orders.Models;
using Microsoft.EntityFrameworkCore;

namespace EatUp.Orders.Repositories
{
    public class Repository<TEntity> : IBaseRepository<TEntity> where TEntity : BaseEntity
    {
        private readonly Context _context;
        public Repository(Context context)
        {
            _context = context;
        }
        public async Task<PaginationResult<TTo>> GetPage<TTo>(int skip, int take, Expression<Func<TEntity, TTo>> mapper, Expression<Func<TEntity, bool>>? filter = null, bool tracking = false, string? orderBy = null, bool ascending = false)
        {
            var query = _context.GetQuery<TEntity>(tracking);

            if (filter != null)
            {
                query = query.Where(filter);
            }
            if (orderBy != null)
            {
                query = query.OrderByColumn(orderBy, ascending);
            }

            var totalCount = query.Count();
            var items = await query.Skip(skip)
                .Take(take)
                .Select(mapper)
                .ToListAsync();

            var result = new PaginationResult<TTo>
            {
                TotalCount = totalCount,
                Items = items,
                Page = (skip / take) + 1
            };

            return result;
        }

        public async Task<TEntity?> GetById(Guid id, bool tracking = false, bool ignoreFilters = false, params Expression<Func<TEntity, object>>[] includes)
        {
            var query = _context.GetQuery(tracking, includes);
            
            if (ignoreFilters)
                query = query.IgnoreQueryFilters();
            
            return await query.FirstOrDefaultAsync(e => e.Id == id);
        }


        public Task<TEntity> Insert(TEntity entity)
        {
            _context.Add(entity);
            return Task.FromResult(entity);
        }

        public Task Delete(Guid id)
        {
            var entity = _context.GetQuery<TEntity>(true).FirstOrDefault(m => m.Id == id);
            if (entity == null)
                throw new ArgumentException("Entity not found");
            _context.Remove(entity);
            return Task.CompletedTask;
        }
        
        public Task Save()
        {
             _context.SaveChanges();
            return Task.CompletedTask;
        }

        public Task UpdateAll(Expression<Func<TEntity, bool>> query, Action<TEntity> action)
        {
            var entities = _context.GetQuery<TEntity>(true).Where(query).ToList();
            foreach (var entity in entities)
            {
                action(entity);
            }

            return Task.CompletedTask;
        }

        public async Task<TEntity?> GetByExpression(Expression<Func<TEntity, bool>> expression, bool tracking = false)
        {
            return await _context.GetQuery<TEntity>(tracking).FirstOrDefaultAsync(expression);
        }
    }
}
