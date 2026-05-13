using Shared.Dtos;
using Shared.Models;

namespace Application.Interfaces
{
    public interface IFinancialOperationService
    {
        Task CreateAsync(FinancialOperationForCreationDto dto, string accountId, CancellationToken cancellationToken = default);
        Task UpdateAsync(Guid id, FinancialOperationForUpdateDto dto, string accountId, CancellationToken cancellationToken = default);
        Task DeleteAsync(Guid id, string accountId, CancellationToken cancellationToken = default);
        Task<FinancialOperationDto?> GetByIdAsync(Guid id, string accountId, CancellationToken cancellationToken = default);
        Task<FinancialOperationDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<IEnumerable<FinancialOperationDto>> GetAllAsync(string accountId, CancellationToken cancellationToken = default);
        Task<PagedList<FinancialOperationDto>> GetAllAsync(string accountId, PaginationParameters parameters, CancellationToken cancellationToken = default);
        Task<IEnumerable<FinancialOperationDto>> GetAllByDateAsync(DateOnly date, string accountId, CancellationToken cancellationToken = default);
        Task<IEnumerable<FinancialOperationDto>> GetAllByDatePeriodAsync(DateOnly startDate, DateOnly endDate, string accountId, CancellationToken cancellationToken = default);
    }
}
