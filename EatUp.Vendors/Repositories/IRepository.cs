using System.Linq.Expressions;
using EatUp.Vendors.Models;

namespace EatUp.Vendors.Repositories
{
    public interface IRepository<TEntity> where TEntity : BaseEntity
    {
        Task<TTo?> GetById<TTo>(Guid id, Expression<Func<TEntity, TTo>> mapper, bool tracking = false, params Expression<Func<TEntity, object>>[] includes);
        Task UpdateAll(Expression<Func<TEntity, bool>> query, Action<TEntity> action);
        Task<TEntity> Insert(TEntity entity);
        Task Delete(Guid id);
        Task<bool> Exist(Expression<Func<TEntity, bool>> query);
        Task Save();
        Task<TEntity?> GetByExpression(Expression<Func<TEntity, bool>> query, bool tracking = false, params Expression<Func<TEntity, object>>[] includes);
        Task<PaginationResult<TTo>> GetPage<TTo>(int skip, int take, Expression<Func<TEntity, TTo>> mapper, Expression<Func<TEntity, bool>>? filter = null, bool tracking = false, string? orderBy = null, bool ascending = false);

        /// <summary>
        /// Get all entities of type TEntity, excludes the filters.
        /// </summary>
        /// <returns></returns>
        Task<IQueryable<TEntity>> GetAll(bool tracking = false);
    }
}