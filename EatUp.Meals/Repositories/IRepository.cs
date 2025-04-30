using System.Linq.Expressions;
using EatUp.Meals.Models;

namespace EatUp.Meals.Repositories
{
    public interface IRepository<TEntity> where TEntity : BaseEntity
    {
        Task<TEntity> GetById(Guid id, bool tracking = false, params Expression<Func<TEntity, object>>[] includes);
        Task<TEntity> Insert(TEntity entity);
        Task Delete(Guid id);
        Task Save();
        Task<IQueryable<TEntity>> GetQuery(bool tracking = false, params Expression<Func<TEntity, object>>[] includes);
        Task<PaginationResult<TTo>> GetPage<TTo>(int skip, int take, Expression<Func<TEntity, TTo>> mapper, Expression<Func<TEntity, bool>>? filter = null, bool tracking = false, string? orderBy = null, bool ascending = false);
        Task<TTo?> GetById<TTo>(Guid id, Expression<Func<TEntity, TTo>> mapper, bool tracking = false, params Expression<Func<TEntity, object>>[] includes);
    }
}
