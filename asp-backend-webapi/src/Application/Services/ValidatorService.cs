using Application.Interfaces;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Application.Services
{
    public class ValidatorService(IServiceProvider serviceProvider) : IValidatorService
    {
        private readonly IServiceProvider _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));

        public Task ValidateAndThrowAsync<T>(T dto, CancellationToken cancellationToken = default) where T : class
        {
            ArgumentNullException.ThrowIfNull(dto);

            var validator = _serviceProvider.GetRequiredService<AbstractValidator<T>>();

            return validator.ValidateAndThrowAsync(dto, cancellationToken);
        }
    }
}
