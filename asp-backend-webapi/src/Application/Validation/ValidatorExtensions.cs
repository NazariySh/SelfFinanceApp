using FluentValidation;

namespace Application.Validation
{
    internal static class ValidatorExtensions
    {
        public static IRuleBuilderOptions<T, string> FirstLetterIsCapital<T>(this IRuleBuilder<T, string> ruleBuilder)
        {
            return ruleBuilder
                    .Matches("^[A-ZА-Я].*").WithMessage("First letter must be capital.");
        }

        public static IRuleBuilderOptions<T, string> ValidateEmail<T>(this IRuleBuilder<T, string> ruleBuilder)
        {
            return ruleBuilder
                    .NotEmpty().WithMessage("Email is required.")
                    .EmailAddress().WithMessage("Email is not valid.");
        }

        public static IRuleBuilderOptions<T, decimal> ValidateAmount<T>(this IRuleBuilder<T, decimal> ruleBuilder)
        {
            return ruleBuilder
                    .NotEmpty().WithMessage("Amount is required")
                    .GreaterThan(0).WithMessage("Amount must be greater than 0");
        }
    }
}
