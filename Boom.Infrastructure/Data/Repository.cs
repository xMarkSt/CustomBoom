using System.Security.Claims;
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

    public async Task<T> CreateAsync<T>(T entity) where T : class, IEntity
    {
        if (entity == null)
        {
            throw new ArgumentNullException("entity");
        }
        
        _dbContext.Add(entity);
        
        await _dbContext.SaveChangesAsync();
        return entity;
    }

    public async Task<IEnumerable<T>> CreateAsync<T>(IEnumerable<T> entity) where T : class, IEntity
    {
        if (entity == null)
        {
            throw new ArgumentNullException("entity");
        }

        foreach(var e in entity)
        {
            _dbContext.Add<T>(e);
        }

        await _dbContext.SaveChangesAsync();
        return entity;
    }

    public async Task<bool> RemoveAsync<TEntity>(TEntity entity) where TEntity : class, IEntity
    {
        ArgumentNullException.ThrowIfNull(entity);

        _dbContext.Remove<TEntity>(entity);
        await _dbContext.SaveChangesAsync();
        return true;
    }
        
    public async Task<bool> RemoveRangeAsync<TEntity>(IEnumerable<TEntity> entity) where TEntity : class, IEntity
    {
        ArgumentNullException.ThrowIfNull(entity);

        _dbContext.RemoveRange(entity);
        await _dbContext.SaveChangesAsync();
        return true;
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

    public async Task<TEntity> UpdateAsync<TEntity>(TEntity entity) where TEntity : class, IEntity
    {
        ArgumentNullException.ThrowIfNull(entity);
        if (entity.Id < 0)
        {
            throw new InvalidOperationException($"id of object of type {entity.GetType()} is not set.");
        }

        _dbContext.Update<TEntity>(entity);
        await _dbContext.SaveChangesAsync();

        return entity;
    }

    public void ClearChanges()
    {
        _dbContext.ChangeTracker.Clear();
    }
}