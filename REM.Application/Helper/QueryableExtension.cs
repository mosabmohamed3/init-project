namespace REM.Application.Helper;

public static class QueryableExtension
{
    public static IQueryable<T> Paginate<T>(this IQueryable<T> query, Pagination pagination)
    {
        return query.Skip((pagination.PageNumber - 1) * pagination.PageSize).Take(pagination.PageSize);
    }
}
