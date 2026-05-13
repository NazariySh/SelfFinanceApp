using Shared.Dtos;

namespace Shared.Models
{
    public record DailyReport(DateOnly Date, decimal TotalIncome, decimal TotalExpense, IReadOnlyCollection<FinancialOperationDto> Operations);
}
