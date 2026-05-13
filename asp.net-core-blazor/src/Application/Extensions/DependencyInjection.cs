using Application.Handlers;
using Application.Interfaces;
using Application.Services;
using Application.States;
using Blazored.SessionStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Application.Profiles;
using Domain.Models;

namespace Application.Extensions
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
        {
            var selfFinanceApiSettings = configuration.GetSection("WebApi:SelfFinanceApi");

            var webApiName = selfFinanceApiSettings["Name"] ?? throw new ArgumentException("SelfFinanceApi name is required");
            var webApiBaseAddress = selfFinanceApiSettings["BaseAddress"] ?? throw new ArgumentException("SelfFinanceApi base address is required");

            services.AddHttpClient(webApiName, client => client.BaseAddress = new Uri(webApiBaseAddress));

            services.Configure<WebApiSettings>(selfFinanceApiSettings);

            services.AddScoped<IProtectedHttpClientProvider, ProtectedHttpClientProvider>();

            services.AddAutoMapper(config =>
            {
                config.AddMaps(typeof(AppUserDtoProfile).Assembly);
            });

            services.AddBlazoredSessionStorage();
            services.AddDataProtection();
            services.AddScoped<IProtectedStorageService, ProtectedStorageService>();

            services.AddScoped<IAuthenticationHandler, AuthenticationHandler>();
            services.AddSingleton<IErrorResponseHandler, ErrorResponseHandler>();

            services.AddScoped<ICustomAuthStateProvider, CustomAuthStateProvider>();
            services.AddScoped<AuthenticationStateProvider, CustomAuthStateProvider>();

            services.AddScoped<IRefreshTokenService, RefreshTokenService>();
            services.AddScoped<IAuthenticationService, AuthenticationService>();
            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<IFinancialOperationService, FinancialOperationService>();
            services.AddScoped<IFinancialReportService, FinancialReportService>();

            return services;
        }
    }
}
