using Shared.Dtos;
using FluentValidation;
using Application.Validation.Common;
using Microsoft.Extensions.DependencyInjection;

namespace Application.Validation
{
    public class AppUserForCreationDtoValidator : AbstractValidator<AppUserForCreationDto>
    {
        public AppUserForCreationDtoValidator(
            [FromKeyedServices(nameof(UserNameValidator))] AbstractValidator<string> userNameValidator,
            [FromKeyedServices(nameof(PasswordValidator))] AbstractValidator<string> passwordValidator)
        {
            RuleFor(x => x.UserName).SetValidator(userNameValidator);

            RuleFor(x => x.Password).SetValidator(passwordValidator);

            RuleFor(x => x.Email).ValidateEmail();

            RuleFor(x => x.ConfirmPassword)
                .NotEmpty()
                .Equal(x => x.Password)
                .WithMessage("Passwords do not match");
        }
    }
}
