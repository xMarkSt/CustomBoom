using Boom.Infrastructure.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Boom.Infrastructure.Data;

public class Repository<TContext> : IRepository where TContext : DbContext
{
    private readonly BoomDbContext _dbContext;

    public Repository(BoomDbContext context)
    {
        _dbContext = context;
    }

    public void Add<T>(T entity) where T : class, IEntity
    {
        ArgumentNullException.ThrowIfNull(entity);
        _dbContext.Add(entity);
    }

    public void AddRange<T>(IEnumerable<T> entities) where T : class, IEntity
    {
        ArgumentNullException.ThrowIfNull(entities);
        _dbContext.AddRange(entities);
    }

    public void Remove<TEntity>(TEntity entity) where TEntity : class, IEntity
    {
        ArgumentNullException.ThrowIfNull(entity);
        _dbContext.Remove(entity);
    }

    public void RemoveRange<TEntity>(IEnumerable<TEntity> entities) where TEntity : class, IEntity
    {
        ArgumentNullException.ThrowIfNull(entities);
        _dbContext.RemoveRange(entities);
    }

    public void Update<TEntity>(TEntity entity) where TEntity : class, IEntity
    {
        ArgumentNullException.ThrowIfNull(entity);
        _dbContext.Update(entity);
    }

    public async Task SaveAsync()
    {
        await _dbContext.SaveChangesAsync();
    }

    public IQueryable<TEntity> GetAll<TEntity>() where TEntity : class, IEntity
    {
        return _dbContext.Set<TEntity>();
    }

    public TEntity? GetById<TEntity>(int id) where TEntity : class, IEntity
    {
        return _dbContext.Set<TEntity>().Find(id);
    }

    public TEntity? GetById<TEntity>(long id) where TEntity : class, IEntity
    {
        return _dbContext.Set<TEntity>().Find(id);
    }

    public void ClearChanges()
    {
        _dbContext.ChangeTracker.Clear();
    }
}
