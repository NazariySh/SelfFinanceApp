using Application.Interfaces;
using Shared.Enums;
using Shared.Models;

namespace Application.Services
{
    public class FinancialReportService(IFinancialOperationService operationService) : IFinancialReportService
    {
        private readonly IFinancialOperationService _operationService = operationService ?? throw new ArgumentNullException(nameof(operationService));

        public async Task<DailyReport> GetDailyReportAsync(
            DateOnly date,
            string accountId,
            CancellationToken cancellationToken = default)
        {
            if (date == default)
            {
                throw new ArgumentException("Date cannot be empty", nameof(date));
            }

            if (string.IsNullOrEmpty(accountId))
            {
                throw new ArgumentException("User Id cannot be empty", nameof(accountId));
            }

            var operations = await _operationService.GetAllByDateAsync(date, accountId, cancellationToken);

            var totalIncome = operations.Where(x => x.Type == OperationType.Income).Sum(x => x.Amount);
            var totalExpenses = operations.Where(x => x.Type == OperationType.Expense).Sum(x => x.Amount);

            return new DailyReport(date, totalIncome, totalExpenses, operations.ToList());
        }

        public async Task<DatePeriodReport> GetDatePeriodReportAsync(
            DateOnly startDate,
            DateOnly endDate,
            string accountId,
            CancellationToken cancellationToken = default)
        {
            if (startDate == default)
            {
                throw new ArgumentException("Start date cannot be empty", nameof(startDate));
            }

            if (endDate == default)
            {
                throw new ArgumentException("End date cannot be empty", nameof(endDate));
            }

            if (startDate > endDate)
            {
                throw new ArgumentException("Start date cannot be greater than end date", nameof(startDate));
            }

            if (string.IsNullOrEmpty(accountId))
            {
                throw new ArgumentException("User Id cannot be empty", nameof(accountId));
            }

            var operations = await _operationService.GetAllByDatePeriodAsync(startDate, endDate, accountId, cancellationToken);

            var totalIncome = operations.Where(x => x.Type == OperationType.Income).Sum(x => x.Amount);
            var totalExpense = operations.Where(x => x.Type == OperationType.Expense).Sum(x => x.Amount);

            return new DatePeriodReport(startDate, endDate, totalIncome, totalExpense, operations.ToList());
        }
    }
}
