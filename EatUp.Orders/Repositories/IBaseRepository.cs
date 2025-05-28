using System.Linq.Expressions;
using EatUp.Orders.Models;

namespace EatUp.Orders.Repositories
{
    public interface IBaseRepository<TEntity> where TEntity : BaseEntity
    {
        Task<PaginationResult<TTo>> GetPage<TTo>(int skip, int take, Expression<Func<TEntity, TTo>> mapper, Expression<Func<TEntity, bool>>? filter = null, bool tracking = false, string? orderBy = null, bool ascending = false); 
        Task UpdateAll(Expression<Func<TEntity, bool>> query, Action<TEntity> action);
        Task<TEntity> GetById(Guid id, bool tracking = false, bool ignoreFilters = false, params Expression<Func<TEntity, object>>[] includes);
        Task<TEntity> Insert(TEntity entity);
        Task Delete(Guid id);
        Task Save();
        Task<TEntity?> GetByExpression(Expression<Func<TEntity, bool>> expression, bool tracking = false);
        int Count(Expression<Func<TEntity, bool>> expression);
    }
}
