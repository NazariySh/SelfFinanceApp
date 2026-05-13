using Shared.Attributes;
using Shared.Enums;
using System.ComponentModel.DataAnnotations;

namespace Shared.Dtos
{
    public class FinancialOperationForUpdateDto
    {
        public Guid Id { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        [NotInFuture]
        public DateTime Date { get; set; }

        [Required]
        public OperationType Type { get; set; }

        [Required]
        public string Description { get; set; } = string.Empty;

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0")]
        public decimal Amount { get; set; }
    }
}
