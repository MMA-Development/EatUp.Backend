using EatUp.Meals.Models;
using EatUp.Meals.Repositories;

namespace EatUp.Meals.Services
{
    public class MealService(IBaseRepository<Meal> repository) : IMealService
    {
        public void AddMeal(Meal meal)
        {
            EnsureMeal(meal);
            repository.Insert(meal);
            repository.Save();
        }

        public async Task<PaginationResult<Meal>> GetPage(int skip, int take)
        {
            return await repository.GetPage(skip, take, null, false);
        }

        public void EnsureMeal(Meal meal)
        {
            if (meal.LastAvailablePickup < DateTime.UtcNow)
            {
                throw new ArgumentException("Last available pickup time cannot be in the past.");
            }
        }

        public async Task Delete(Guid id)
        {
            await repository.Delete(id);
            await repository.Save();
        }
    }
}
