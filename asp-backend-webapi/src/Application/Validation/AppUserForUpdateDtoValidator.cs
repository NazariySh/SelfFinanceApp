using Shared.Dtos;
using FluentValidation;
using Application.Validation.Common;
using Microsoft.Extensions.DependencyInjection;

namespace Application.Validation
{
    public class AppUserForUpdateDtoValidator : AbstractValidator<AppUserForUpdateDto>
    {
        public AppUserForUpdateDtoValidator(
            [FromKeyedServices(nameof(UserNameValidator))] AbstractValidator<string> userNameValidator)
        {
            RuleFor(x => x.UserName).SetValidator(userNameValidator);

            RuleFor(x => x.Email).ValidateEmail();
        }
    }
}
