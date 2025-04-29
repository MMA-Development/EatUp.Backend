using System.Linq.Expressions;
using EatUp.Meals.Models;

namespace EatUp.Meals.Repositories
{
    public interface IBaseRepository<TEntity> where TEntity : BaseEntity
    {
        Task<PaginationResult<TEntity>> GetPage(int skip, int take, Expression<Func<TEntity, bool>>? filter = null, bool tracking = false);
        Task UpdateAll(Expression<Func<TEntity, bool>> query, Action<TEntity> action);
        Task<TEntity> GetById(Guid id, bool tracking = false, params Expression<Func<TEntity, object>>[] includes);
        Task<TEntity> Insert(TEntity entity);
        Task Delete(Guid id);
        Task Save();
    }
}
