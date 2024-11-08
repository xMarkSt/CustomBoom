using Boom.Infrastructure.Data.Entities;

namespace Boom.Infrastructure.Data;

public interface IRepository
{
    Task<T> CreateAsync<T>(T entity) where T : class, IEntity;
    Task<IEnumerable<T>> CreateAsync<T>(IEnumerable<T> entity) where T : class, IEntity;
    Task<bool> RemoveAsync<TEntity>(TEntity entity) where TEntity : class, IEntity;
    Task<bool> RemoveRangeAsync<TEntity>(IEnumerable<TEntity> entity) where TEntity : class, IEntity;
    IQueryable<TEntity> GetAll<TEntity>() where TEntity : class, IEntity;
    TEntity? GetById<TEntity>(int id) where TEntity : class, IEntity;
    TEntity? GetById<TEntity>(long id) where TEntity : class, IEntity;
    Task<TEntity> UpdateAsync<TEntity>(TEntity entity) where TEntity : class, IEntity;
    void ClearChanges();
}