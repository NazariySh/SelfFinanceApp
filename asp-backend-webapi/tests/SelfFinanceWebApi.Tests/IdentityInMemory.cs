using Application.Interfaces;
using Application.Services;
using Domain.Entities;
using Domain.Models;
using Infrastructure.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Shared.Enums;
using System.Text;

namespace SelfFinanceWebApi.Tests
{
    public class IdentityInMemory
    {
        public IServiceProvider ServiceProvider { get; }

        private readonly string[] _roles;

        public IdentityInMemory(string databaseName)
        {
            var services = new ServiceCollection();

            var configuration = new ConfigurationBuilder()
                    .AddJsonFile("testsettings.json")
                    .Build();

            _roles = Enum.GetNames(typeof(RoleType));

            services.AddSingleton<IConfiguration>(configuration);

            services.AddLogging();

            services.Configure<JwtSettings>(configuration.GetSection(nameof(JwtSettings)));
            services.AddScoped<IJwtTokenService, JwtTokenService>();

            services.AddDbContext<FinancialDbContext>(options => options.UseInMemoryDatabase(databaseName));

            services.AddIdentity<AppUser, IdentityRole>()
                .AddEntityFrameworkStores<FinancialDbContext>()
                .AddRoles<IdentityRole>()
                .AddUserManager<UserManager<AppUser>>();

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.SaveToken = true;
                options.RequireHttpsMetadata = false;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = configuration["JwtSettings:Issuer"],
                    ValidAudience = configuration["JwtSettings:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JwtSettings:Key"]!))
                };
            });

            ServiceProvider = services.BuildServiceProvider();

            CreateRolesAsync(ServiceProvider);
        }

        private async void CreateRolesAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();

            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            foreach (var role in _roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }
        }
    }
}
