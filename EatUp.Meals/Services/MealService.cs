using System.Linq.Expressions;
using EatUp.Meals.DTO;
using EatUp.Meals.Extensions;
using EatUp.Meals.Models;
using EatUp.Meals.Repositories;
using EatUp.RabbitMQ.Events.Meals;
using Microsoft.AspNetCore.Mvc;

namespace EatUp.Meals.Services
{
    public class MealService(
        IRepository<Meal> repository,
        IRepository<VendorProjection> vendorProjections,
        IRabbitMqPublisher publisher) : IMealService
    {
        public async Task<Guid> AddMeal(Guid vendorId, AddMealDTO addMealDTO)
        {
            var vendor = await vendorProjections.GetById(vendorId);
            EnsureMeal(addMealDTO, vendor);
            Meal meal = addMealDTO.ToMeal(vendorId, vendor.Name);
            await repository.Insert(meal);
            await repository.Save();
            var mealCreatedEvent = ToMealCreatedEvent(meal);
            await publisher.Publish(mealCreatedEvent);
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

        private void EnsureMeal(AddMealDTO meal, VendorProjection vendor)
        {
            ArgumentNullException.ThrowIfNull(meal);
            ArgumentNullException.ThrowIfNull(vendor);
            if (meal.LastAvailablePickup < DateTime.UtcNow)
            {
                throw new ArgumentException("Last available pickup time cannot be in the past.");
            }
        }

        public async Task Delete(Guid mealId, Guid vendorId)
        {
            var meal = await repository.GetById(mealId);
            if (meal.VendorId != vendorId)
                throw new Exception("Could not delete meal. Meal does not belong to vendor.");

            await repository.Delete(mealId);
            await repository.Save();

            var @event = new MealDeletedEvent(mealId);
            await publisher.Publish(@event);
        }

        public async Task UpdateMeal(Guid mealId, Guid vendorId, UpdateMealDTO updateMealDTO)
        {
            var meal = await repository.GetById(mealId, true);
            if (meal.VendorId != vendorId)
                throw new Exception("Could not delete meal. Meal does not belong to vendor.");

            updateMealDTO.MergeMeal(meal);
            await repository.Save();
            
            var @event = ToUpdateEvent(meal);
            await publisher.Publish(@event);
        }

        private MealUpdatedEvent ToUpdateEvent(Meal meal) => new()
        {
            Title = meal.Title,
            Id = meal.Id,
            Description = meal.Description,
        };
        
        private MealCreatedEvent ToMealCreatedEvent(Meal meal) => new()
        {
            Description = meal.Description,
            Id = meal.Id,
            Title = meal.Title
        };

        public async Task<MealDTO> GetMeal(Guid mealId)
        {
            var meal = await repository.GetById(mealId, MealDTO.FromMeal, includes: [x => x.Categories]);
            return meal;
        }
    }
}
