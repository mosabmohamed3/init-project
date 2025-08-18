using Microsoft.EntityFrameworkCore;
using REM.Application.Common.Interfaces;
using REM.Domain.Entities;

namespace REM.Infrastructure.Context;

public class UnitOfWork<TContext>(TContext context) : IUnitOfWork<TContext>, IUnitOfWork
    where TContext : DbContext
{
    private readonly Dictionary<Type, object> _repositories = [];

    public TContext Context { get; } = context;

    public void Dispose()
    {
        Context.Dispose();
        _repositories.Clear();
    }

    public IGenericRepository<T> GetRepository<T>()
        where T : BaseEntity
    {
        var type = typeof(T);
        if (!_repositories.ContainsKey(type))
            _repositories[type] = new GenericRepository<T, TContext>(Context);
        return (IGenericRepository<T>)_repositories[type];
    }

    public Task<int> SaveChangesAsync()
    {
        return Context.SaveChangesAsync();
    }
}
