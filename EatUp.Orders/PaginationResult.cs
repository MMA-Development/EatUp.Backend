using EatUp.Orders.Models;

namespace EatUp.Orders
{
    public class PaginationResult<TEntity>
    {
        public IEnumerable<TEntity> Items { get; set; }
        public int TotalCount { get; set; }
        public int Page { get; set; }
    }
}
