using Domain.Entities;
using Shared.Models;
using System.Linq.Expressions;

namespace Domain.Repository
{
    public interface IFinancialOperationRepository : IAbstractRepository<FinancialOperation>
    {
        Task<IEnumerable<FinancialOperation>> GetAllAsync(string accountId, CancellationToken cancellationToken = default);
        Task<PagedList<FinancialOperation>> GetAllAsync(string accountId, PaginationParameters parameters, CancellationToken cancellationToken = default);
        Task<IEnumerable<FinancialOperation>> WhereAsync(Expression<Func<FinancialOperation, bool>> predicate, string accountId, CancellationToken cancellationToken = default);
    }
}
