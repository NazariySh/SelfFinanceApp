using Application.Interfaces;
using Application.Profiles;
using Application.Services;
using Application.Validation;
using Shared.Dtos;
using Shared.Enums;
using Domain.Exceptions;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using SelfFinanceWebApi.Tests.Factory;
using SelfFinanceWebApi.Tests.Interfaces;
using Application.Validation.Common;

namespace SelfFinanceWebApi.Tests.Application.Tests.Services.Tests
{
    public class FinancialOperationServiceTests
    {
        private readonly IFinancialOperationServiceFactory _repositoryServiceFactory;

        public FinancialOperationServiceTests()
        {
            var serviceProvider = ConfigureServices();

            _repositoryServiceFactory = serviceProvider.GetRequiredService<IFinancialOperationServiceFactory>();
        }

        [Fact]
        public async Task CreateAsync_ShouldThrowValidationException_WhenDtoIsInvalid()
        {
            var service = _repositoryServiceFactory.CreateService("CreateAsyncFinancialOperationService2");

            var dto = new FinancialOperationForCreationDto
            {
                Date = DateTime.UtcNow,
                Type = OperationType.Income,
                Description = " ",
                Amount = -5000
            };

            await Assert.ThrowsAsync<ValidationException>(() => service.CreateAsync(dto, "1"));
        }

        [Fact]
        public async Task AddAsync_ShouldThrowOperationAlreadyExistException_WhenEntityAlreadyExists()
        {
            var service = _repositoryServiceFactory.CreateService("CreateAsyncFinancialOperationService3");

            var dto = new FinancialOperationForCreationDto
            {
                Date = DateTime.UtcNow,
                Type = OperationType.Income,
                Description = "Test",
                Amount = 5000
            };

            await service.CreateAsync(dto, "1");

            await Assert.ThrowsAsync<OperationAlreadyExistException>(() => service.CreateAsync(dto, "1"));
        }

        [Fact]
        public async Task CreateAsync_ShouldAddAsync_WhenDtoIsValid()
        {
            var service = _repositoryServiceFactory.CreateService("CreateAsyncFinancialOperationService4");

            var userId = "1";

            var dto = new FinancialOperationForCreationDto
            {
                Date = DateTime.UtcNow,
                Type = OperationType.Income,
                Description = "Test",
                Amount = 5000
            };

            await service.CreateAsync(dto, userId);

            var operation = await service.GetByIdAsync(dto.Id);

            Assert.NotNull(operation);
            Assert.Equal(dto.Description, operation.Description);
            Assert.Equal(dto.Amount, operation.Amount);
            Assert.Equal(userId, operation.UserId);
        }

        [Fact]
        public async Task UpdateAsync_ShouldThrowArgumentException_WhenIdDoesNotMatch()
        {
            var service = _repositoryServiceFactory.CreateService("UpdateAsyncFinancialOperationService1");

            var dto = new FinancialOperationForUpdateDto
            {
                Id = Guid.NewGuid(),
                Date = DateTime.UtcNow,
                Type = OperationType.Income,
                Description = "Test",
                Amount = 5000
            };

            await Assert.ThrowsAsync<ArgumentException>(() => service.UpdateAsync(Guid.NewGuid(), dto, "1"));
        }

        [Fact]
        public async Task UpdateAsync_ShouldThrowValidationException_WhenDtoIsInvalid()
        {
            var service = _repositoryServiceFactory.CreateService("UpdateAsyncFinancialOperationService2");

            var dto = new FinancialOperationForUpdateDto
            {
                Id = Guid.NewGuid(),
                Date = DateTime.UtcNow,
                Type = OperationType.Income,
                Description = " ",
                Amount = -5000
            };

            await Assert.ThrowsAsync<ValidationException>(() => service.UpdateAsync(dto.Id, dto, "1"));
        }

        [Fact]
        public async Task UpdateAsync_ShouldThrowOperationNotFoundException_WhenOperationDoesNotExist()
        {
            var service = _repositoryServiceFactory.CreateService("UpdateAsyncFinancialOperationService3");

            var dto = new FinancialOperationForUpdateDto
            {
                Id = Guid.NewGuid(),
                Date = DateTime.UtcNow,
                Type = OperationType.Income,
                Description = "Test",
                Amount = 5000
            };

            await Assert.ThrowsAsync<OperationNotFoundException>(() => service.UpdateAsync(dto.Id, dto, "1"));
        }

        [Fact]
        public async Task UpdateAsync_ShouldThrowOperationForbiddenException_WhenUserIsNotOwner()
        {
            var service = _repositoryServiceFactory.CreateService("UpdateAsyncFinancialOperationService4");

            var dto = new FinancialOperationForCreationDto
            {
                Date = DateTime.UtcNow,
                Type = OperationType.Income,
                Description = "Test",
                Amount = 5000
            };

            await service.CreateAsync(dto, "2");

            var dto2 = new FinancialOperationForUpdateDto
            {
                Id = dto.Id,
                Date = DateTime.UtcNow,
                Type = OperationType.Income,
                Description = "Test",
                Amount = 5000
            };

            await Assert.ThrowsAsync<OperationForbiddenException>(() => service.UpdateAsync(dto.Id, dto2, "1"));
        }

        [Fact]
        public async Task UpdateAsync_ShouldThrowOperationAlreadyExistException_WhenEntityAlreadyExists()
        {
            var service = _repositoryServiceFactory.CreateService("UpdateAsyncFinancialOperationService5");

            var userId = "1";

            var dto = new FinancialOperationForCreationDto
            {
                Date = DateTime.UtcNow,
                Type = OperationType.Income,
                Description = "Test",
                Amount = 5000
            };

            await service.CreateAsync(dto, userId);

            var dto2 = new FinancialOperationForCreationDto
            {
                Date = DateTime.UtcNow,
                Type = OperationType.Expense,
                Description = "Test2",
                Amount = 1000
            };

            await service.CreateAsync(dto2, userId);

            var dto3 = new FinancialOperationForUpdateDto
            {
                Id = dto.Id,
                Date = dto2.Date,
                Type = OperationType.Expense,
                Description = "Test2",
                Amount = 1000
            };

            await Assert.ThrowsAsync<OperationAlreadyExistException>(() => service.UpdateAsync(dto.Id, dto3, userId));
        }

        [Fact]
        public async Task UpdateAsync_ShouldThrowOperationNotFoundException_WhenOperationIdNotFound()
        {
            var service = _repositoryServiceFactory.CreateService("UpdateAsyncFinancialOperationService6");

            var userId = "1";

            var dto = new FinancialOperationForCreationDto
            {
                Date = DateTime.UtcNow,
                Type = OperationType.Income,
                Description = "Test",
                Amount = 5000
            };

            await service.CreateAsync(dto, userId);

            var dto2 = new FinancialOperationForUpdateDto
            {
                Id = Guid.NewGuid(),
                Date = DateTime.UtcNow,
                Type = OperationType.Expense,
                Description = "Test2",
                Amount = 1000
            };

            await Assert.ThrowsAsync<OperationNotFoundException>(() => service.UpdateAsync(dto2.Id, dto2, userId));
        }

        [Fact]
        public async Task UpdateAsync_ShouldUpdateAsync_WhenDtoIsValid()
        {
            var service = _repositoryServiceFactory.CreateService("UpdateAsyncFinancialOperationService7");

            var userId = "1";

            var dto = new FinancialOperationForCreationDto
            {
                Date = DateTime.UtcNow,
                Type = OperationType.Income,
                Description = "Test",
                Amount = 5000
            };

            await service.CreateAsync(dto, userId);

            var dto2 = new FinancialOperationForUpdateDto
            {
                Id = dto.Id,
                Date = DateTime.UtcNow,
                Type = OperationType.Expense,
                Description = "Test2",
                Amount = 1000
            };

            await service.UpdateAsync(dto.Id, dto2, userId);

            var operation = await service.GetByIdAsync(dto.Id);

            Assert.NotNull(operation);
            Assert.Equal(dto2.Description, operation.Description);
            Assert.Equal(dto2.Amount, operation.Amount);
            Assert.Equal(userId, operation.UserId);
        }

        [Fact]
        public async Task DeleteAsync_ThrowOperationNotFoundException_WhenOperationDoesNotExist()
        {
            var service = _repositoryServiceFactory.CreateService("DeleteAsyncFinancialOperationService1");

            await Assert.ThrowsAsync<OperationNotFoundException>(() => service.DeleteAsync(Guid.NewGuid(), "1"));
        }

        [Fact]
        public async Task DeleteAsync_ThrowOperationForbiddenException_WhenUserIsNotOwner()
        {
            var service = _repositoryServiceFactory.CreateService("DeleteAsyncFinancialOperationService2");

            var dto = new FinancialOperationForCreationDto
            {
                Date = DateTime.UtcNow,
                Type = OperationType.Income,
                Description = "Test",
                Amount = 5000
            };

            await service.CreateAsync(dto, "2");

            await Assert.ThrowsAsync<OperationForbiddenException>(() => service.DeleteAsync(dto.Id, "1"));
        }

        [Fact]
        public async Task DeleteAsync_ShouldDeleteAsync_WhenOperationExists()
        {
            var service = _repositoryServiceFactory.CreateService("DeleteAsyncFinancialOperationService3");

            var userId = "1";

            var dto = new FinancialOperationForCreationDto
            {
                Date = DateTime.UtcNow,
                Type = OperationType.Income,
                Description = "Test",
                Amount = 5000
            };

            await service.CreateAsync(dto, userId);

            await service.DeleteAsync(dto.Id, userId);

            var operation = await service.GetByIdAsync(dto.Id);

            Assert.Null(operation);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnNull_WhenOperationDoesNotExist()
        {
            var service = _repositoryServiceFactory.CreateService("GetByIdAsyncFinancialOperationService1");

            var operation = await service.GetByIdAsync(Guid.NewGuid());

            Assert.Null(operation);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnNull_WhenUserIsNotOwner()
        {
            var service = _repositoryServiceFactory.CreateService("GetByIdAsyncFinancialOperationService2");

            var dto = new FinancialOperationForCreationDto
            {
                Date = DateTime.UtcNow,
                Type = OperationType.Income,
                Description = "Test",
                Amount = 5000
            };

            await service.CreateAsync(dto, "2");

            var result = await service.GetByIdAsync(dto.Id, "1");

            Assert.Null(result);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnOperation_WhenOperationExists()
        {
            var service = _repositoryServiceFactory.CreateService("GetByIdAsyncFinancialOperationService3");

            var userId = "1";

            var dto = new FinancialOperationForCreationDto
            {
                Date = DateTime.UtcNow,
                Type = OperationType.Income,
                Description = "Test",
                Amount = 5000
            };

            await service.CreateAsync(dto, userId);

            var operation = await service.GetByIdAsync(dto.Id);

            Assert.NotNull(operation);
            Assert.Equal(dto.Description, operation.Description);
            Assert.Equal(dto.Amount, operation.Amount);
            Assert.Equal(userId, operation.UserId);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnEmptyList_WhenNoOperationsExist()
        {
            var service = _repositoryServiceFactory.CreateService("GetAllAsyncFinancialOperationService1");

            var operations = await service.GetAllAsync("3");

            Assert.Empty(operations);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnEmptyList_WhenNoOperationsExistForUser()
        {
            var service = _repositoryServiceFactory.CreateService("GetAllAsyncFinancialOperationService2");

            var dto = new FinancialOperationForCreationDto
            {
                Date = DateTime.UtcNow,
                Type = OperationType.Income,
                Description = "Test",
                Amount = 5000
            };

            await service.CreateAsync(dto, "2");

            var operations = await service.GetAllAsync("1");

            Assert.Empty(operations);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnOperations_WhenOperationsExist()
        {
            var service = _repositoryServiceFactory.CreateService("GetAllAsyncFinancialOperationService3");

            var dto = new FinancialOperationForCreationDto
            {
                Date = DateTime.UtcNow,
                Type = OperationType.Income,
                Description = "Test",
                Amount = 5000
            };

            await service.CreateAsync(dto, "1");

            var operations = await service.GetAllAsync("1");

            Assert.NotEmpty(operations);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnOperations_WhenOperationsExistForUser()
        {
            var service = _repositoryServiceFactory.CreateService("GetAllAsyncFinancialOperationService4");

            var userId = "1";

            var dto = new FinancialOperationForCreationDto
            {
                Date = DateTime.UtcNow,
                Type = OperationType.Income,
                Description = "Test",
                Amount = 5000
            };

            await service.CreateAsync(dto, userId);

            var operations = await service.GetAllAsync(userId);

            Assert.NotEmpty(operations);
        }

        [Fact]
        public async Task GetAllByDateAsync_ShouldReturnEmptyList_WhenNoOperationsExistForUser()
        {
            var service = _repositoryServiceFactory.CreateService("GetAllByDateAsyncFinancialOperationService1");

            var date = DateTime.UtcNow;

            var dto = new FinancialOperationForCreationDto
            {
                Date = date,
                Type = OperationType.Income,
                Description = "Test",
                Amount = 5000
            };

            await service.CreateAsync(dto, "2");

            var operations = await service.GetAllByDateAsync(DateOnly.FromDateTime(date), "1");

            Assert.Empty(operations);
        }

        [Fact]
        public async Task GetAllByDateAsync_ShouldReturnOperations_WhenOperationsExistForUser()
        {
            var service = _repositoryServiceFactory.CreateService("GetAllByDateAsyncFinancialOperationService2");

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

            var operations = await service.GetAllByDateAsync(DateOnly.FromDateTime(date), userId);

            Assert.NotEmpty(operations);
        }

        [Fact]
        public async Task GetAllByDatePeriodAsync_ShouldThrowArgumentException_WhenStartDateIsGreaterThanEndDate()
        {
            var service = _repositoryServiceFactory.CreateService("GetAllByDatePeriodAsyncFinancialOperationService1");

            var startDate = DateOnly.FromDateTime(DateTime.UtcNow);
            var endDate = startDate.AddDays(-1);

            await Assert.ThrowsAsync<ArgumentException>(() => service.GetAllByDatePeriodAsync(startDate, endDate, "1"));
        }

        [Fact]
        public async Task GetAllByDatePeriodAsync_ShouldReturnEmptyList_WhenNoOperationsExistForUser()
        {
            var service = _repositoryServiceFactory.CreateService("GetAllByDatePeriodAsyncFinancialOperationService2");

            var startDate = DateTime.UtcNow;
            var endDate = startDate.AddDays(1);

            var dto = new FinancialOperationForCreationDto
            {
                Date = startDate,
                Type = OperationType.Income,
                Description = "Test",
                Amount = 5000
            };

            await service.CreateAsync(dto, "2");

            var operations = await service.GetAllByDatePeriodAsync(DateOnly.FromDateTime(startDate), DateOnly.FromDateTime(endDate), "1");

            Assert.Empty(operations);
        }

        [Fact]
        public async Task GetAllByDatePeriodAsync_ShouldReturnOperations_WhenOperationsExistForUser()
        {
            var service = _repositoryServiceFactory.CreateService("GetAllByDatePeriodAsyncFinancialOperationService3");

            var startDate = DateTime.UtcNow;
            var endDate = startDate.AddDays(1);
            var userId = "1";

            var dto = new FinancialOperationForCreationDto
            {
                Date = startDate,
                Type = OperationType.Income,
                Description = "Test",
                Amount = 5000
            };

            await service.CreateAsync(dto, userId);

            var operations = await service.GetAllByDatePeriodAsync(DateOnly.FromDateTime(startDate), DateOnly.FromDateTime(endDate), userId);

            Assert.NotEmpty(operations);
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

            return services.BuildServiceProvider();
        }
    }
}
