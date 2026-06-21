using Boom.Infrastructure.Data.Entities;

namespace Boom.Infrastructure.Data;

public interface IRepository
{
    void Add<T>(T entity) where T : class, IEntity;
    void AddRange<T>(IEnumerable<T> entities) where T : class, IEntity;
    void Remove<TEntity>(TEntity entity) where TEntity : class, IEntity;
    void RemoveRange<TEntity>(IEnumerable<TEntity> entities) where TEntity : class, IEntity;
    void Update<TEntity>(TEntity entity) where TEntity : class, IEntity;
    Task SaveAsync();
    IQueryable<TEntity> GetAll<TEntity>() where TEntity : class, IEntity;
    TEntity? GetById<TEntity>(int id) where TEntity : class, IEntity;
    TEntity? GetById<TEntity>(long id) where TEntity : class, IEntity;
    void ClearChanges();
}
