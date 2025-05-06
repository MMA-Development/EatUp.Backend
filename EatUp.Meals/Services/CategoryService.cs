using EatUp.Meals.DTO;
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

        public async Task<Guid> Create(AddCategoryDTO addCategoryDTO)
        {
            var category = addCategoryDTO.ToCategory();
            var createdCategory = await repository.Insert(category);
            await repository.Save();
            return createdCategory.Id;
        }
    }
}
