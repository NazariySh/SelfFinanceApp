using FluentValidation;
using System.Text.RegularExpressions;

namespace Application.Validation.Common
{
    public partial class PasswordValidator : AbstractValidator<string>
    {
        public PasswordValidator()
        {
            RuleFor(x => x)
                .NotEmpty().WithMessage("Password is required.")
                .Matches(PasswordPattern())
                    .WithMessage("Password must contain at least one uppercase letter, one lowercase letter, one digit, one special character, and be at least 8 characters long.");
        }

        [GeneratedRegex("^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)(?=.*[^\\da-zA-Z]).{8,50}$")]
        private partial Regex PasswordPattern();
    }
}
