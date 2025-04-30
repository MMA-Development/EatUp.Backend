using System.Linq.Expressions;
using System.Reflection;

namespace EatUp.Meals.Extensions
{
    public static class IQueryableExtensions
    {
        public static IQueryable<T> OrderByColumn<T>(this IQueryable<T> source, string columnName, bool ascending = true)
        {
            if (string.IsNullOrEmpty(columnName))
                throw new ArgumentException("Column name must be provided", nameof(columnName));

            var parameter = Expression.Parameter(typeof(T), "x");
            var property = typeof(T).GetProperty(columnName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

            if (property == null)
                throw new ArgumentException($"No such property: {columnName} on type {typeof(T).Name}");

            var propertyAccess = Expression.MakeMemberAccess(parameter, property);

            var orderByExpression = Expression.Lambda(propertyAccess, parameter);

            var methodName = ascending ? "OrderBy" : "OrderByDescending";
            var resultExpression = Expression.Call(
                typeof(Queryable), methodName,
                new[] { typeof(T), property.PropertyType },
                source.Expression, Expression.Quote(orderByExpression));

            return source.Provider.CreateQuery<T>(resultExpression);
        }
    }
}
