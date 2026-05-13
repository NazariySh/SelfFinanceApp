using Shared.Models;

namespace Application.Interfaces
{
    public interface IFinancialReportService
    {
        Task<DailyReport> GetDailyReportAsync(DateOnly date, string accountId, CancellationToken cancellationToken = default);
        Task<DatePeriodReport> GetDatePeriodReportAsync(DateOnly startDate, DateOnly endDate, string accountId, CancellationToken cancellationToken = default);
    }
}
