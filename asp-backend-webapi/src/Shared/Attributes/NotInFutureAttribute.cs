using System.ComponentModel.DataAnnotations;

namespace Shared.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class NotInFutureAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is DateTime date && date > DateTime.Now)
            {
                return new ValidationResult("Date cannot be in the future");
            }

            return ValidationResult.Success;
        }
    }
}
