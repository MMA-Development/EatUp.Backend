using System.Linq.Expressions;
using EatUp.Meals.Models;

namespace EatUp.Meals.DTO
{
    public class CategoryDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; }

        public static Expression<Func<Category, CategoryDTO>> FromCategory => category => new CategoryDTO
        {
            Id = category.Id,
            Name = category.Name
        };
    }
}
