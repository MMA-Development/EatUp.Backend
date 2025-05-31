using System.Linq.Expressions;
using EatUp.Meals.Models;

namespace EatUp.Meals.Repositories
{
    public interface IRepository<TEntity> where TEntity : BaseEntity
    {
        Task<TEntity> GetById(Guid id, bool tracking = false, bool ignoreFilters = false, params Expression<Func<TEntity, object>>[] includes);
        Task<TEntity> Insert(TEntity entity);
        Task Delete(Guid id);
        Task Save();
        Task<IQueryable<TEntity>> GetQuery(bool tracking = false, params Expression<Func<TEntity, object>>[] includes);
        Task<PaginationResult<TTo>> GetPage<TTo>(int skip, int take, Expression<Func<TEntity, TTo>> mapper, Expression<Func<TEntity, bool>>? filter = null, bool tracking = false, string? orderBy = null, bool ascending = false, Expression<Func<TEntity, object>>[]? includes = null);
        Task<TTo?> GetById<TTo>(Guid id, Expression<Func<TEntity, TTo>> mapper, bool tracking = false, params Expression<Func<TEntity, object>>[] includes);

        /// <summary>
        /// Get all entities of type TEntity. ignoring soft delete
        /// </summary>
        /// <param name="tracking"></param>
        /// <returns></returns>
        Task<IQueryable<TEntity>> GetAll(bool tracking = false);
    }
}
