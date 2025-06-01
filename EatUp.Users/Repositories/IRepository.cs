using System.Linq.Expressions;
using EatUp.Users.Models;

namespace EatUp.Users.Repositories
{
    public interface IRepository<TEntity> where TEntity : BaseEntity
    {
        Task<PaginationResult<TEntity>> GetPage(int skip, int take, Expression<Func<TEntity, bool>>? filter = null, bool tracking = false);
        Task UpdateAll(Expression<Func<TEntity, bool>> query, Action<TEntity> action);
        Task<TEntity> GetById(Guid id, bool tracking = false, params Expression<Func<TEntity, object>>[] includes);
        Task<TEntity> Insert(TEntity entity);
        Task Delete(Guid id);
        Task<bool> Exist(Expression<Func<TEntity, bool>> query);
        Task Save();
        Task<TEntity?> GetByExpression(Expression<Func<TEntity, bool>> query, bool tracking = false, params Expression<Func<TEntity, object>>[] includes);

        /// <summary>
        /// Get all entities of type TEntity, excludes the filters.
        /// </summary>
        /// <param name="tracking"></param>
        /// <returns></returns>
        Task<IQueryable<TEntity>> GetAll(bool tracking = false);
        Task<TTo?> GetById<TTo>(Guid id, Expression<Func<TEntity, TTo>> mapper, bool tracking = false, params Expression<Func<TEntity, object>>[] includes);
    }
}
