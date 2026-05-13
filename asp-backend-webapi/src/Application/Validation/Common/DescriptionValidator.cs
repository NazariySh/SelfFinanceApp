using FluentValidation;
using System.Text.RegularExpressions;

namespace Application.Validation.Common
{
    public partial class DescriptionValidator : AbstractValidator<string>
    {
        public DescriptionValidator()
        {
            RuleFor(x => x)
                .NotEmpty().WithMessage("Description is required")
                .MaximumLength(255).WithMessage("Description must be less than 255 characters")
                .FirstLetterIsCapital().WithMessage("Description must start with a capital letter.")
                .Matches(ValidDescriptionCharactersPattern()).WithMessage("Description contains invalid characters.");
        }

        [GeneratedRegex("^[\\p{L}\\p{M}0-9,.\\s\\-'`():!#?\"]+$")]
        private partial Regex ValidDescriptionCharactersPattern();
    }
}
