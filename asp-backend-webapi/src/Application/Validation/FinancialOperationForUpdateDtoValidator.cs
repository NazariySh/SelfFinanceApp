using Shared.Dtos;
using FluentValidation;
using Application.Validation.Common;
using Microsoft.Extensions.DependencyInjection;

namespace Application.Validation
{
    public class FinancialOperationForUpdateDtoValidator : AbstractValidator<FinancialOperationForUpdateDto>
    {
        public FinancialOperationForUpdateDtoValidator(
            [FromKeyedServices(nameof(DateValidator))] AbstractValidator<DateTime> dateValidator,
            [FromKeyedServices(nameof(DescriptionValidator))] AbstractValidator<string> descriptionValidator)
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Id is required.");

            RuleFor(x => x.Date).SetValidator(dateValidator);

            RuleFor(x => x.Description).SetValidator(descriptionValidator);

            RuleFor(x => x.Amount).ValidateAmount();
        }
    }
}
