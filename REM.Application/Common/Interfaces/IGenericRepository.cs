using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Query;
using REM.Domain.Dto;

namespace REM.Application.Common.Interfaces;

public interface IGenericRepository<TEntity>
{
    void Remove(TEntity entity);

    void RemoveRange(IEnumerable<TEntity> entites);

    void Update(TEntity entity);

    void UpdateRange(IEnumerable<TEntity> entites);

    void Delete(TEntity entity);

    Task AddAsync(TEntity entity);

    Task AddRangeAsync(IEnumerable<TEntity> entites);

    Task<TEntity?> FindAsync(
        Expression<Func<TEntity, bool>> filterPredicate,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? Include = null,
        Expression<Func<TEntity, TEntity>>? select = null,
        bool asNoTracking = false,
        bool asSplit = false,
        bool IgnoreFilter = false,
        bool withDeleted = false
    );


    Task<TResult?> FindAsync<TResult>(
        Expression<Func<TEntity, bool>> filterPredicate,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? Include = null,
        bool asNoTracking = false,
        Expression<Func<TEntity, TResult>>? select = null,
        bool asSplit = false,
        bool IgnoreFilter = false,
        bool withDeleted = false
    );

    public Task<TResult?> FindAsync<TResult>(
        Expression<Func<TEntity, bool>> filterPredicate,
        IConfigurationProvider projectToConfig,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? Include = null,
        bool asNoTracking = false,
        bool asSplit = false,
        bool IgnoreFilter = false,
        bool withDeleted = false
    );

    Task<List<TEntity>> GetAllAsync(
        Expression<Func<TEntity, bool>> filterPredicate,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? Include = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
        Expression<Func<TEntity, TEntity>>? select = null,
        int? take = null,
        bool ignoreFilter = false,
        bool asSplit = false,
        bool asNoTracking = true
    );

    Task<List<TResult>> GetAllAsync<TResult>(
        Expression<Func<TEntity, bool>> filterPredicate,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object?>>? Include = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
        Expression<Func<TEntity, TResult>>? select = null,
        int? take = null,
        bool ignoreFilter = false,
        bool asSplit = false,
        bool asNoTracking = true
    );

    Task<List<TResult>> GetAllAsync<TResult>(
        Expression<Func<TEntity, bool>> filterPredicate,
        IConfigurationProvider projectToConfig,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object?>>? Include = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
        int? take = null,
        bool ignoreFilter = false,
        bool asSplit = false,
        bool asNoTracking = true
    );

    Task<List<TEntity>> GetAllAsync(
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null
    );

    Task<IQueryable<TEntity>> GetAllQueryableAsync(
        Expression<Func<TEntity, bool>>? filterPredicate = null,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? Include = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
        bool withDeleted = false,
        bool asSplit = false,
        bool asNoTracking = false
    );

    Task<bool> IsExistsAsync(Expression<Func<TEntity, bool>>? filterPredicate = null);

    Task<bool> IsExistsAsync(IEnumerable<Guid> ids);

    Task<(List<TEntity>, int)> GetPaginatedAsync(
        Expression<Func<TEntity, bool>>? filterPredicate = null,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? Include = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
        Pagination? pagentation = null,
        Expression<Func<TEntity, TEntity>>? select = null
    );

    IQueryable<TEntity> GetPaginatedQuerableAsync(
        Expression<Func<TEntity, bool>>? filterPredicate = null,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? Include = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
        Pagination? pagentation = null,
        Expression<Func<TEntity, TEntity>>? select = null,
        bool withDelted = false
    );

    Task<int> CountAsync(
        Expression<Func<TEntity, bool>>? filterPredicate = null,
        bool withDeleted = false
    );

    Task<decimal> SumAsync(
        Expression<Func<TEntity?, decimal>> sum,
        Expression<Func<TEntity?, bool>>? filter = null
    );

    Task<int> SaveChangesAsync();
}
