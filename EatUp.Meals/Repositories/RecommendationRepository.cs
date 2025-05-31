using EatUp.Meals.DTO;
using EatUp.Meals.Models;
using Microsoft.EntityFrameworkCore;

namespace EatUp.Meals.Repositories
{
    public class RecommendationRepository(Context _context) : IRecommendationRepository
    {
        public async Task<PaginationResult<MealDTO>> GetRecommendedMealsForUser(Guid userId, int skip, int take)
        {
            var meals = _context.GetQuery<Meal>(includes: [x => x.Categories]).IgnoreQueryFilters();
            var mostBoughtCategory = await _context.GetQuery<CompletedOrderProjection>()
                .Where(m => m.UserId == userId)
                .Join(meals, order => order.MealId, m => m.Id, (order, meal) => meal.Categories)
                .SelectMany(x => x)
                .GroupBy(c => c.Id)
                .Select(g => new { CategoryId = g.Key, Count = g.Count() })
                .OrderByDescending(x => x.Count)
                .FirstOrDefaultAsync();

            var recommendedMeals = _context.GetQuery<CompletedOrderProjection>()
                .Join(meals, order => order.MealId, m => m.Id, (order, meal) => new
                {
                    MealId = meal.Id,
                    MealCategories = meal.Categories,
                })
                .Where(x => mostBoughtCategory != null && x.MealCategories.Any(x => x.Id == mostBoughtCategory.CategoryId) || true)
                .GroupBy(x => x.MealId)
                .Select(m => new
                {
                    MealId = m.Key,
                    Count = m.Count()
                })
                .OrderByDescending(x => x.Count);

            var query = meals.Where(x => recommendedMeals.Any(r => r.MealId == x.Id) && x.LastAvailablePickup > DateTime.UtcNow && x.DeletedAt == null && x.CompletedOrders.Sum(y => y.Quantity) < x.Quantity );
            var total = query.Count();
            var recommended = await query
                .Skip(skip)
                .Take(take)
                .Select(MealDTO.FromMeal)
                .ToListAsync();


            return new PaginationResult<MealDTO>
            {
                TotalCount = total,
                Items = recommended,
                Page = (skip / take) + 1
            };
        }
    }
}
