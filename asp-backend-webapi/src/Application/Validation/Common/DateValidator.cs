using FluentValidation;

namespace Application.Validation.Common
{
    public class DateValidator : AbstractValidator<DateTime>
    {
        public DateValidator()
        {
            RuleFor(x => x)
                .NotEmpty().WithMessage("Date is required")
                .GreaterThan(new DateTime(1991, 1, 1)).WithMessage("Date must be after 1991-01-01");
        }
    }
}
