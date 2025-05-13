using EatUp.Vendors.Models;

namespace EatUp.Vendors
{
    public class PaginationResult<TEntity>
    {
        public IEnumerable<TEntity> Items { get; set; }
        public int TotalCount { get; set; }
        public int Page { get; set; }
    }
}
