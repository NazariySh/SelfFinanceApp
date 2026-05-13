using Domain.Entities;
using System.Linq.Expressions;

namespace Domain.Repository
{
    public interface IAbstractRepository<T> where T : BaseEntity
    {
        Task AddAsync(T entity, CancellationToken cancellationToken = default);
        Task UpdateAsync(T entity, CancellationToken cancellationToken = default);
        Task RemoveAsync(T entity, CancellationToken cancellationToken = default);
        Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<IEnumerable<T>> WhereAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);
    }
}
