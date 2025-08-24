using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using UserManagerDemo.Application.Common.Interface;
using UserManagerDemo.Domain.Common;
using UserManagerDemo.Infrastructure.Persistence;

namespace UserManagerDemo.Infrastructure.Repositories;

public class Repository<TEntity, TPrimaryKey> : IRepository<TEntity, TPrimaryKey>

    where TEntity : class, IEntity<TPrimaryKey>

{
    private readonly AppDbContext _context;
    private readonly DbSet<TEntity> _dbSet;

    public Repository(AppDbContext context)
    {
        _context = context;
        _dbSet = context.Set<TEntity>();
    }

    public async Task AddAsync(TEntity entity) => await _dbSet.AddAsync(entity);

    public Task DeleteAsync(TEntity entity)
    {
        _dbSet.Remove(entity);
        return Task.CompletedTask;
    }

    public async Task<IReadOnlyList<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate)
        => await _dbSet.Where(predicate).ToListAsync();

    public async Task<IReadOnlyList<TEntity>> GetAllAsync() => await _dbSet.ToListAsync();

    public async Task<TEntity?> GetByIdAsync(TPrimaryKey id) => await _dbSet.FindAsync(id);

    public async Task<(IEnumerable<TEntity> Items, int TotalCount)> GetPagedAsync(int pageIndex, int pageSize)
    {
        var totalCount = await _dbSet.CountAsync();
        var items = await _dbSet.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
        return (items, totalCount);
    }

    public Task UpdateAsync(TEntity entity)
    {
        _dbSet.Update(entity);
        return Task.CompletedTask;
    }
}