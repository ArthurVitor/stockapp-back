using System.Linq.Expressions;

namespace StockApp.API.Extensions;

public static class QueryableExtensions
{
    public static IQueryable<T> OrderByField<T>(this IQueryable<T> query, string sortingField, bool isAscending = true)
    {
        if (string.IsNullOrWhiteSpace(sortingField))
        {
            return query; // No sorting if the sort field is not provided
        }
        
        var param = Expression.Parameter(typeof(T), "p");
        var property = Expression.Property(param, sortingField);
        var sortingExpression = Expression.Lambda(property, param);

        string methodType = isAscending ? "OrderBy" : "OrderByDescending";
        MethodCallExpression orderByCall = Expression.Call(
            typeof(Queryable),
            methodType,
            [typeof(T), property.Type],
            query.Expression,
            Expression.Quote(sortingExpression));

        return query.Provider.CreateQuery<T>(orderByCall);
    }
}