using EatUp.Meals.Models;

namespace EatUp.Meals.Services
{
    public interface ICategoryService
    {
        Task<PaginationResult<Category>> GetPage(int skip, int take);
    }
}