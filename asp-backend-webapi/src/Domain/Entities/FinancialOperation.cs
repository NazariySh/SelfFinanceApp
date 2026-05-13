using Domain.Enums;

namespace Domain.Entities
{
    public class FinancialOperation : BaseEntity
    {
        public DateTime Date { get; set; }
        public OperationType Type { get; set; }
        public string Description { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string UserId { get; set; } = string.Empty;
    }
}
