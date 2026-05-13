using Shared.Enums;

namespace Shared.Dtos
{
    public class FinancialOperationDto : BaseDto
    {
        public DateTime Date { get; set; }
        public OperationType Type { get; set; }
        public string Description { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string UserId { get; set; } = string.Empty;
    }
}
