using System.Linq.Expressions;
using UserManagerDemo.Domain.Common;

namespace UserManagerDemo.Application.Common.Interface;

public interface IRepository<TEntity, TPrimaryKey> where TEntity : class, IEntity<TPrimaryKey>
{
    Task AddAsync(TEntity entity);

    Task DeleteAsync(TEntity entity);

    Task<IReadOnlyList<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate);

    Task<IReadOnlyList<TEntity>> GetAllAsync();

    Task<TEntity?> GetByIdAsync(TPrimaryKey id);

    Task<(IEnumerable<TEntity> Items, int TotalCount)> GetPagedAsync(int pageIndex, int pageSize);

    Task UpdateAsync(TEntity entity);
}