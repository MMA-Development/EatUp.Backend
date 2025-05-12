using System.Linq.Expressions;

namespace EatUp.Orders.Extensions
{
    public static class ExpressionExtensions
    {
        public static Expression<Func<T, bool>> AndAll<T>(this List<Expression<Func<T, bool>>> expressions)
        {
            if (expressions == null || !expressions.Any())
            {
                return x => true;
            }

            var parameter = Expression.Parameter(typeof(T), "x");
            Expression combined = null;

            foreach (var expr in expressions)
            {
                // Replace the parameter in each expression with the shared parameter
                var body = new ReplaceParameterVisitor(expr.Parameters[0], parameter).Visit(expr.Body);

                combined = combined == null ? body : Expression.AndAlso(combined, body);
            }

            return Expression.Lambda<Func<T, bool>>(combined, parameter);
        }

        private class ReplaceParameterVisitor : ExpressionVisitor
        {
            private readonly ParameterExpression _oldParam;
            private readonly ParameterExpression _newParam;

            public ReplaceParameterVisitor(ParameterExpression oldParam, ParameterExpression newParam)
            {
                _oldParam = oldParam;
                _newParam = newParam;
            }

            protected override Expression VisitParameter(ParameterExpression node)
            {
                return node == _oldParam ? _newParam : base.VisitParameter(node);
            }
        }
    }
}
