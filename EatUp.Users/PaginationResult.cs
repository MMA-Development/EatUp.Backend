using EatUp.Users.Models;

namespace EatUp.Users
{
    public class PaginationResult<TEntity> where TEntity: BaseEntity
    {
        public IEnumerable<TEntity> Items { get; set; }
        public int TotalCount { get; set; }
        public int Page { get; set; }
    }
}
