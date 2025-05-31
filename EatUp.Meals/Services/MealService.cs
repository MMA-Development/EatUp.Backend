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
        IRabbitMqPublisher publisher,
        IRepository<Category> categoryRepository,
        IRepository<Review> reviewRepository,
        IRepository<UserFavoriteMealsProjection> userFavoriteMealsRepository,
        IRepository<CompletedOrderProjection> orderProjections) : IMealService
    {
        public async Task<Guid> AddMeal(Guid vendorId, AddMealDTO addMealDTO)
        {
            var vendor = await vendorProjections.GetById(vendorId);
            EnsureMeal(addMealDTO, vendor);
            var categories = (await categoryRepository.GetQuery(true)).Where(x => addMealDTO.Categories.Contains(x.Id)).ToList();
            Meal meal = addMealDTO.ToMeal(vendorId, vendor.Name, categories);
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

            filters.Add(m => m.LastAvailablePickup >= DateTime.UtcNow);
            filters.Add(m => m.Quantity - m.CompletedOrders.Sum(x => x.Quantity) > 0);
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
            meal.Categories = (await categoryRepository.GetQuery(true))
                .Where(x => updateMealDTO.Categories.Contains(x.Id))
                .ToList();
            await repository.Save();
            
            var @event = ToUpdateEvent(meal);
            await publisher.Publish(@event);
        }

        private MealUpdatedEvent ToUpdateEvent(Meal meal) => new()
        {
            Title = meal.Title,
            Id = meal.Id,
            Description = meal.Description,
            Quantity = meal.Quantity
        };
        
        private MealCreatedEvent ToMealCreatedEvent(Meal meal) => new()
        {
            Description = meal.Description,
            Id = meal.Id,
            Title = meal.Title,
            Quantity = meal.Quantity
        };

        public async Task<MealDTO> GetMeal(Guid mealId)
        {
            var meal = await repository.GetById(mealId, MealDTO.FromMeal, includes: [x => x.Categories]);
            return meal;
        }

        public async Task AddReview(Guid mealId, AddReviewDTO reviewDto, Guid userId)
        {
            var userHasACompletedOrderForMeal = (await orderProjections.GetQuery()).Any(x => x.UserId == userId && x.MealId == mealId);
            if(!userHasACompletedOrderForMeal)
            {
                throw new InvalidOperationException("User has not completed an order for this meal. Cannot add review.");
            }
            var review = reviewDto.ToReview(mealId, userId);
            await reviewRepository.Insert(review);
            await reviewRepository.Save();
        }

        public async Task<PaginationResult<MealDTO>> GetFavorites(Guid userId, int skip, int take)
        {
            return await repository.GetPage(skip, take, MealDTO.FromMeal, x => x.UserFavorites.Any(y => y.UserId == userId && y.DeletedAt == null), includes: [x => x.UserFavorites]);
        }
    }
}
