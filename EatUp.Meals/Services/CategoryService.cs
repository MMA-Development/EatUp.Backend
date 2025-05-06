using EatUp.Meals.Models;
using EatUp.Meals.Repositories;

namespace EatUp.Meals.Services
{
    public class CategoryService(IRepository<Category> repository) : ICategoryService
    {
        public async Task<PaginationResult<Category>> GetPage(int skip, int take)
        {
            return await repository.GetPage(skip, take, c => c);
        }
    }
}
