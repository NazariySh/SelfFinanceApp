using Application.Interfaces;
using Application.Profiles;
using Application.Services;
using Application.Validation;
using Shared.Dtos;
using Shared.Enums;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using SelfFinanceWebApi.Tests.Factory;
using SelfFinanceWebApi.Tests.Interfaces;
using Application.Validation.Common;

namespace SelfFinanceWebApi.Tests.Application.Tests.Services.Tests
{
    public class FinancialReportServiceTests
    {
        private readonly IFinancialOperationServiceFactory _repositoryServiceFactory;
        private readonly IFinancialReportServiceFactory _financialReportService;

        public FinancialReportServiceTests()
        {
            var serviceProvider = ConfigureServices();

            _repositoryServiceFactory = serviceProvider.GetRequiredService<IFinancialOperationServiceFactory>();
            _financialReportService = serviceProvider.GetRequiredService<IFinancialReportServiceFactory>();
        }

        [Fact]
        public async Task GetReportAsync_ShouldReturnReport_WhenOperationsExists()
        {
            var dbContextFactory = new DbContextFactoryInMemory("GetReportAsyncFinancialReportService3");
            var service = _repositoryServiceFactory.CreateService(dbContextFactory);
            var reportService = _financialReportService.CreateService(dbContextFactory);

            var date = DateTime.UtcNow;
            var userId = "1";

            var dto = new FinancialOperationForCreationDto
            {
                Date = date,
                Type = OperationType.Income,
                Description = "Test",
                Amount = 5000
            };

            await service.CreateAsync(dto, userId);

            var dto2 = new FinancialOperationForCreationDto
            {
                Date = date,
                Type = OperationType.Expense,
                Description = "Test descriptions2",
                Amount = 5000
            };

            await service.CreateAsync(dto2, userId);

            var report = await reportService.GetDailyReportAsync(DateOnly.FromDateTime(dto.Date), userId);

            Assert.NotNull(report);
            Assert.Equal(DateOnly.FromDateTime(dto.Date), report.Date);
            Assert.Equal(dto.Amount, report.TotalIncome);
            Assert.Equal(dto2.Amount, report.TotalExpense);
            Assert.Equal(2, report.Operations.Count);
        }

        [Fact]
        public async Task GetReportAsync_ShouldReturnEmptyReport_WhenOperationsDoesNotExist()
        {
            var dbContextFactory = new DbContextFactoryInMemory("GetReportAsyncFinancialReportService4");
            var reportService = _financialReportService.CreateService(dbContextFactory);

            var report = await reportService.GetDailyReportAsync(DateOnly.FromDateTime(DateTime.UtcNow), "1");

            Assert.NotNull(report);
            Assert.Equal(0, report.TotalIncome);
            Assert.Equal(0, report.TotalExpense);
            Assert.Empty(report.Operations);
        }

        [Fact]
        public async Task GetDatePeriodReportAsync_ThrowArgumentException_WhenStartDateIsInTheFuture()
        {
            var dbContextFactory = new DbContextFactoryInMemory("GetDatePeriodReportAsyncFinancialReportService3");
            var reportService = _financialReportService.CreateService(dbContextFactory);

            await Assert.ThrowsAsync<ArgumentException>(() => reportService.GetDatePeriodReportAsync(DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1)), DateOnly.FromDateTime(DateTime.UtcNow), "1"));
        }

        [Fact]
        public async Task GetDatePeriodReportAsync_ThrowArgumentException_WhenStartDateIsGreaterThanEndDate()
        {
            var dbContextFactory = new DbContextFactoryInMemory("GetDatePeriodReportAsyncFinancialReportService5");
            var reportService = _financialReportService.CreateService(dbContextFactory);

            await Assert.ThrowsAsync<ArgumentException>(() => reportService.GetDatePeriodReportAsync(DateOnly.FromDateTime(DateTime.UtcNow), new DateOnly(1991, 1, 1), "1"));
        }

        [Fact]
        public async Task GetDatePeriodReportAsync_ShouldReturnReport_WhenOperationsExists()
        {
            var dbContextFactory = new DbContextFactoryInMemory("GetDatePeriodReportAsyncFinancialReportService6");
            var service = _repositoryServiceFactory.CreateService(dbContextFactory);
            var reportService = _financialReportService.CreateService(dbContextFactory);

            var userId = "1";

            var dto = new FinancialOperationForCreationDto
            {
                Date = DateTime.UtcNow.AddDays(-10),
                Type = OperationType.Income,
                Description = "Test",
                Amount = 5000
            };

            await service.CreateAsync(dto, userId);

            var dto2 = new FinancialOperationForCreationDto
            {
                Date = DateTime.UtcNow,
                Type = OperationType.Expense,
                Description = "Test descriptions2",
                Amount = 5000
            };

            await service.CreateAsync(dto2, userId);

            var dto3 = new FinancialOperationForCreationDto
            {
                Date = DateTime.UtcNow,
                Type = OperationType.Expense,
                Description = "Test descriptions3",
                Amount = 10000
            };

            await service.CreateAsync(dto3, userId);

            var report = await reportService.GetDatePeriodReportAsync(DateOnly.FromDateTime(dto.Date), DateOnly.FromDateTime(dto3.Date), userId);

            Assert.NotNull(report);
            Assert.Equal(DateOnly.FromDateTime(dto.Date), report.StartDate);
            Assert.Equal(DateOnly.FromDateTime(dto3.Date), report.EndDate);
            Assert.Equal(dto.Amount, report.TotalIncome);
            Assert.Equal(dto2.Amount + dto3.Amount, report.TotalExpense);
            Assert.Equal(3, report.Operations.Count);
        }

        private static ServiceProvider ConfigureServices()
        {
            var services = new ServiceCollection();

            services.AddLogging();

            services.AddAutoMapper(cfg =>
            {
                cfg.AddProfile<FinancialOperationProfile>();
            });

            services.AddKeyedSingleton<AbstractValidator<string>, DescriptionValidator>(nameof(DescriptionValidator));
            services.AddKeyedSingleton<AbstractValidator<DateTime>, DateValidator>(nameof(DateValidator));

            services.AddSingleton<AbstractValidator<FinancialOperationForCreationDto>, FinancialOperationForCreationDtoValidator>();
            services.AddSingleton<AbstractValidator<FinancialOperationForUpdateDto>, FinancialOperationForUpdateDtoValidator>();

            services.AddScoped<IValidatorService, ValidatorService>();

            services.AddScoped<IFinancialOperationRepositoryFactory, FinancialOperationRepositoryFactory>();

            services.AddScoped<IFinancialOperationServiceFactory, FinancialOperationServiceFactory>();
            services.AddScoped<IFinancialReportServiceFactory, FinancialReportServiceFactory>();

            return services.BuildServiceProvider();
        }
    }
}
