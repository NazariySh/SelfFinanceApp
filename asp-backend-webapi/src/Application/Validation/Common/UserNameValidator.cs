using FluentValidation;
using System.Text.RegularExpressions;

namespace Application.Validation.Common
{
    public partial class UserNameValidator : AbstractValidator<string>
    {
        public UserNameValidator()
        {
            RuleFor(x => x)
                .NotEmpty().WithMessage("Username is required.")
                .MinimumLength(2).WithMessage("Username must be at least 2 characters long.")
                .MaximumLength(50).WithMessage("Username must not exceed 50 characters.")
                .Matches(UserNamePattern()).WithMessage("Username must contain only letters and numbers.");
        }

        [GeneratedRegex("^[a-zA-Z0-9-]+$")]
        private partial Regex UserNamePattern();
    }
}
