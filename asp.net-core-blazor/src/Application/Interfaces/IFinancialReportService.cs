using Shared.Models;

namespace Application.Interfaces
{
    public interface IFinancialReportService
    {
        Task<DailyReport?> GetDailyReportAsync(DateOnly date, CancellationToken cancellationToken = default);
        Task<DatePeriodReport?> GetDatePeriodReportAsync(DateOnly startDate, DateOnly endDate, CancellationToken cancellationToken = default);
    }
}
