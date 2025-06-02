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


            var recommendMeals = _context.GetQuery<Meal>().Where(m => m.Categories.Any(x => mostBoughtCategory != null && m.Categories.Any(c => c.Id == mostBoughtCategory.CategoryId) || true));

            var query = recommendMeals.Where(x => x.LastAvailablePickup > DateTime.UtcNow && x.DeletedAt == null && x.CompletedOrders.Sum(y => y.Quantity) < x.Quantity )
                .OrderByDescending(x => x.CompletedOrders);
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
