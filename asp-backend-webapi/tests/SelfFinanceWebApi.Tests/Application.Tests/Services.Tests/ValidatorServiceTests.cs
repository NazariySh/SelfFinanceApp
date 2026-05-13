using Application.Interfaces;
using Application.Services;
using Application.Validation;
using Shared.Dtos;
using Shared.Enums;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Application.Validation.Common;

namespace SelfFinanceWebApi.Tests.Application.Tests.Services.Tests
{
    public class ValidatorServiceTests
    {
        private readonly IValidatorService _validatorService;

        public ValidatorServiceTests()
        {
            var serviceProvider = ConfigureServices();

            _validatorService = serviceProvider.GetRequiredService<IValidatorService>();
        }

        [Fact]
        public async Task ValidateAndThrowAsync_WhenDtoIsValid_ShouldNotThrowException()
        {
            var dto = new FinancialOperationForCreationDto
            {
                Date = DateTime.UtcNow,
                Type = OperationType.Income,
                Description = "Some operations description",
                Amount = 5000
            };

            await _validatorService.ValidateAndThrowAsync(dto);

            Assert.True(true);
        }

        [Fact]
        public async Task ValidateAndThrowAsync_WhenDtoIsInvalid_ShouldThrowValidationException()
        {
            var dto = new FinancialOperationForCreationDto
            {
                Date = DateTime.UtcNow,
                Type = OperationType.Income,
                Description = "",
                Amount = -5000
            };

            await Assert.ThrowsAsync<ValidationException>(() => _validatorService.ValidateAndThrowAsync(dto));
        }

        private static ServiceProvider ConfigureServices()
        {
            var services = new ServiceCollection();

            services.AddKeyedSingleton<AbstractValidator<string>, UserNameValidator>(nameof(UserNameValidator));
            services.AddKeyedSingleton<AbstractValidator<string>, PasswordValidator>(nameof(PasswordValidator));
            services.AddKeyedSingleton<AbstractValidator<string>, DescriptionValidator>(nameof(DescriptionValidator));
            services.AddKeyedSingleton<AbstractValidator<DateTime>, DateValidator>(nameof(DateValidator));

            services.AddSingleton<AbstractValidator<FinancialOperationForCreationDto>, FinancialOperationForCreationDtoValidator>();
            services.AddSingleton<AbstractValidator<FinancialOperationForUpdateDto>, FinancialOperationForUpdateDtoValidator>();
            services.AddSingleton<AbstractValidator<AppUserForCreationDto>, AppUserForCreationDtoValidator>();
            services.AddSingleton<AbstractValidator<AppUserForUpdateDto>, AppUserForUpdateDtoValidator>();

            services.AddScoped<IValidatorService, ValidatorService>();

            return services.BuildServiceProvider();
        }
    }
}
