using EatUp.Meals.Models;

namespace EatUp.Meals
{
    public class PaginationResult<TEntity>
    {
        public IEnumerable<TEntity> Items { get; set; }
        public int TotalCount { get; set; }
        public int Page { get; set; }
    }
}
