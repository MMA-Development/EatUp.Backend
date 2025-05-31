using EatUp.Meals.DTO;
using EatUp.Meals.Extensions;
using EatUp.Meals.Models;
using EatUp.Meals.Repositories;

namespace EatUp.Meals.Services
{
    public class CategoryService(IRepository<Category> repository) : ICategoryService
    {
        public async Task<PaginationResult<CategoryDTO>> GetPage(int skip, int take)
        {
            return await repository.GetPage(skip, take, CategoryDTO.FromCategory);
        }

        public async Task<Guid> Create(AddCategoryDTO addCategoryDTO)
        {
            var category = addCategoryDTO.ToCategory();
            var existing = (await repository.GetQuery()).FirstOrDefault(c => c.Name == category.Name);
            existing.IfNotNull(() => throw new ArgumentException($"Category with name {category.Name} already exists."));
            var createdCategory = await repository.Insert(category);
            await repository.Save();
            return createdCategory.Id;
        }
    }
}
