using System.Linq.Expressions;
using EatUp.Users.Models;
using Microsoft.EntityFrameworkCore;

namespace EatUp.Users.Repositories
{
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : BaseEntity
    {
        private readonly Context _context;
        public Repository(Context context)
        {
            _context = context;
        }
        public async Task<PaginationResult<TEntity>> GetPage(int skip, int take, Expression<Func<TEntity, bool>>? filter = null, bool tracking = false)
        {
            var query = _context.GetQuery<TEntity>(tracking);

            if (filter != null)
            {
                query = query.Where(filter);
            }

            var totalCount = query.Count();
            var items = await query.Skip(skip).Take(take).ToListAsync();
            var result = new PaginationResult<TEntity>
            {
                TotalCount = totalCount,
                Items = items
            };

            return result;
        }

        public async Task<TEntity?> GetById(Guid id, bool tracking = false, params Expression<Func<TEntity, object>>[] includes)
        {
            return await _context.GetQuery(tracking, includes).FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task<TTo?> GetById<TTo>(Guid id, Expression<Func<TEntity, TTo>> mapper, bool tracking = false, params Expression<Func<TEntity, object>>[] includes)
        {
            return await _context.GetQuery(tracking, includes).Where(x => x.Id == id).Select(mapper).FirstOrDefaultAsync();
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

        public Task<bool> Exist(Expression<Func<TEntity, bool>> query)
        {
            return Task.FromResult(_context.Set<TEntity>().Any(query));
        }

        public async Task<TEntity?> GetByExpression(Expression<Func<TEntity, bool>> query, bool tracking = false, params Expression<Func<TEntity, object>>[] includes)
        {
            return await _context.GetQuery(tracking, includes).FirstOrDefaultAsync(query);
        }

        public async Task<IQueryable<TEntity>> GetAll(bool tracking = false)
        {
            return  _context.GetQuery<TEntity>(tracking).IgnoreQueryFilters();
        }
    }
}
