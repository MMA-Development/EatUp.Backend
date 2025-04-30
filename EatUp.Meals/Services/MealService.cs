using System.Linq.Expressions;
using EatUp.Meals.DTO;
using EatUp.Meals.Extensions;
using EatUp.Meals.Models;
using EatUp.Meals.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace EatUp.Meals.Services
{
    public class MealService(IRepository<Meal> repository) : IMealService
    {
        public async Task<Guid> AddMeal(Guid vendorId, AddMealDTO addMealDTO)
        {
            Meal meal = addMealDTO.ToMeal(vendorId);
            EnsureMeal(meal);
            await repository.Insert(meal);
            await repository.Save();
            return meal.Id;
        }

        public async Task<PaginationResult<MealDTO>> GetPage(MealSearchParamsDTO mealSearchParams)
        {
            var filter = BuildPaginationFilter(mealSearchParams);
            var mealsPage = await repository.GetPage(mealSearchParams.Skip, mealSearchParams.Take, MealDTO.FromMeal, filter, false, mealSearchParams.SortBy, mealSearchParams.Ascending);

            return mealsPage;
        }

        private Expression<Func<Meal, bool>> BuildPaginationFilter(MealSearchParamsDTO mealSearchParams)
        {
            var filters = new List<Expression<Func<Meal, bool>>>();

            if (mealSearchParams.VendorId != null)
            {
                filters.Add(m => m.VendorId == mealSearchParams.VendorId);
            }

            if (mealSearchParams.Search != null)
            {
                filters.Add(m => m.Title.Contains(mealSearchParams.Search) || m.Description.Contains(mealSearchParams.Search));
            }

            if (mealSearchParams.Categories != null)
            {
                filters.Add(m => m.Categories.Any(c => mealSearchParams.Categories.Contains(c.Id)));
            }

            return filters.AndAll();
        }

        public void EnsureMeal(Meal meal)
        {
            if (meal.LastAvailablePickup < DateTime.UtcNow)
            {
                throw new ArgumentException("Last available pickup time cannot be in the past.");
            }
        }

        public async Task Delete(Guid mealId, Guid vendorId)
        {
            var meal = await repository.GetById(mealId);
            if(meal.VendorId != vendorId)
                throw new Exception("Could not delete meal. Meal does not belong to vendor.");

            await repository.Delete(mealId);
            await repository.Save();
        }
    }
}
