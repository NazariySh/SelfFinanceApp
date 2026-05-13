using Shared.Dtos;

namespace Shared.Models
{
    public record DatePeriodReport(DateOnly StartDate, DateOnly EndDate, decimal TotalIncome, decimal TotalExpense, IReadOnlyCollection<FinancialOperationDto> Operations);
}
