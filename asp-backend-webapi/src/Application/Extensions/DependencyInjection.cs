using Application.Interfaces;
using Application.Profiles;
using Application.Services;
using Application.Validation;
using Application.Validation.Common;
using Domain.Models;
using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shared.Dtos;

namespace Application.Extensions
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAutoMapper(config =>
            {
                config.AddMaps(typeof(FinancialOperationProfile).Assembly);
            });

            services.AddKeyedSingleton<AbstractValidator<string>, UserNameValidator>(nameof(UserNameValidator));
            services.AddKeyedSingleton<AbstractValidator<string>, PasswordValidator>(nameof(PasswordValidator));
            services.AddKeyedSingleton<AbstractValidator<string>, DescriptionValidator>(nameof(DescriptionValidator));
            services.AddKeyedSingleton<AbstractValidator<DateTime>, DateValidator>(nameof(DateValidator));

            services.AddSingleton<AbstractValidator<FinancialOperationForCreationDto>, FinancialOperationForCreationDtoValidator>();
            services.AddSingleton<AbstractValidator<FinancialOperationForUpdateDto>, FinancialOperationForUpdateDtoValidator>();
            services.AddSingleton<AbstractValidator<AppUserForCreationDto>, AppUserForCreationDtoValidator>();
            services.AddSingleton<AbstractValidator<AppUserForUpdateDto>, AppUserForUpdateDtoValidator>();

            services.Configure<JwtSettings>(configuration.GetSection(nameof(JwtSettings)));
            services.AddScoped<IJwtTokenService, JwtTokenService>();

            services.AddScoped<IValidatorService, ValidatorService>();
            services.AddScoped<IFinancialOperationService, FinancialOperationService>();
            services.AddScoped<IFinancialReportService, FinancialReportService>();
            services.AddScoped<IAccountService, AccountService>();

            return services;
        }
    }
}
