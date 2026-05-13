using Application.Interfaces;
using AutoMapper;
using Shared.Dtos;
using Shared.Models;
using Domain.Entities;
using Domain.Exceptions;
using Domain.Repository;

namespace Application.Services
{
    public class FinancialOperationService(
        IFinancialOperationRepository operationRepository,
        IValidatorService validatorService,
        IMapper mapper)
        : IFinancialOperationService
    {
        private readonly IFinancialOperationRepository _operationRepository = operationRepository ?? throw new ArgumentNullException(nameof(operationRepository));
        private readonly IValidatorService _validatorService = validatorService ?? throw new ArgumentNullException(nameof(validatorService));
        private readonly IMapper _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));

        public async Task CreateAsync(FinancialOperationForCreationDto dto, string accountId, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(dto);

            if (string.IsNullOrEmpty(accountId))
            {
                throw new ArgumentException("User Id cannot be empty", nameof(accountId));
            }

            await _validatorService.ValidateAndThrowAsync(dto, cancellationToken);

            var newOperation = _mapper.Map<FinancialOperation>(dto);
            newOperation.UserId = accountId;

            await _operationRepository.AddAsync(newOperation, cancellationToken);
        }

        public async Task UpdateAsync(Guid id, FinancialOperationForUpdateDto dto, string accountId, CancellationToken cancellationToken = default)
        {
            if (id == Guid.Empty)
            {
                throw new ArgumentException("Id cannot be empty", nameof(id));
            }

            ArgumentNullException.ThrowIfNull(dto);

            if (string.IsNullOrEmpty(accountId))
            {
                throw new ArgumentException("User Id cannot be empty", nameof(accountId));
            }

            if (id != dto.Id)
            {
                throw new ArgumentException("Id does not match the operation id", nameof(dto));
            }

            await _validatorService.ValidateAndThrowAsync(dto, cancellationToken);

            var operationForUpdate = await _operationRepository.GetByIdAsync(id, cancellationToken)
                ?? throw new OperationNotFoundException($"Financial operation with id {id} does not exist!");

            if (operationForUpdate.UserId != accountId)
            {
                throw new OperationForbiddenException("You are not allowed to update this operation");
            }

            _mapper.Map(dto, operationForUpdate);

            await _operationRepository.UpdateAsync(operationForUpdate, cancellationToken);
        }

        public async Task DeleteAsync(Guid id, string accountId, CancellationToken cancellationToken = default)
        {
            if (id == Guid.Empty)
            {
                throw new ArgumentException("Id cannot be empty", nameof(id));
            }

            if (string.IsNullOrEmpty(accountId))
            {
                throw new ArgumentException("User Id cannot be empty", nameof(accountId));
            }

            var operation = await _operationRepository.GetByIdAsync(id, cancellationToken)
                ?? throw new OperationNotFoundException($"Financial operation with id {id} does not exist!");

            if (operation.UserId != accountId)
            {
                throw new OperationForbiddenException("You are not allowed to delete this operation");
            }

            await _operationRepository.RemoveAsync(operation, cancellationToken);
        }

        public async Task<FinancialOperationDto?> GetByIdAsync(Guid id, string accountId, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(accountId))
            {
                throw new ArgumentException("User Id cannot be empty", nameof(accountId));
            }

            var operation = await GetByIdAsync(id, cancellationToken);

            if (operation is null || operation.UserId != accountId)
            {
                return null;
            }

            return operation;
        }

        public async Task<FinancialOperationDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            if (id == Guid.Empty)
            {
                throw new ArgumentException("Id cannot be empty", nameof(id));
            }

            var operation = await _operationRepository.GetByIdAsync(id, cancellationToken);

            return operation is null ? null : _mapper.Map<FinancialOperationDto>(operation);
        }

        public async Task<IEnumerable<FinancialOperationDto>> GetAllAsync(string accountId, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(accountId))
            {
                throw new ArgumentException("User Id cannot be empty", nameof(accountId));
            }

            var operations = await _operationRepository.GetAllAsync(accountId, cancellationToken);

            var sortedOperations = operations.OrderByDescending(x => x.Date);

            return _mapper.Map<IEnumerable<FinancialOperationDto>>(sortedOperations);
        }

        public async Task<PagedList<FinancialOperationDto>> GetAllAsync(string accountId, PaginationParameters parameters, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(accountId))
            {
                throw new ArgumentException("User Id cannot be empty", nameof(accountId));
            }

            ArgumentNullException.ThrowIfNull(parameters);

            var pagedOperations = await _operationRepository.GetAllAsync(accountId, parameters, cancellationToken);

            var operationDtos = _mapper.Map<List<FinancialOperationDto>>(pagedOperations.Items);

            return new PagedList<FinancialOperationDto>(operationDtos, pagedOperations.PageNumber, pagedOperations.PageSize, pagedOperations.TotalCount);
        }

        public async Task<IEnumerable<FinancialOperationDto>> GetAllByDateAsync(
            DateOnly date,
            string accountId,
            CancellationToken cancellationToken = default)
        {
            if (date == default)
            {
                throw new ArgumentException("Date cannot be empty", nameof(date));
            }

            if (string.IsNullOrEmpty(accountId))
            {
                throw new ArgumentException("User Id cannot be empty", nameof(accountId));
            }

            var operations = await _operationRepository.WhereAsync(x =>
                DateOnly.FromDateTime(x.Date) == date, accountId, cancellationToken);

            var sortedOperations = operations.OrderByDescending(x => x.Date);

            return _mapper.Map<IEnumerable<FinancialOperationDto>>(sortedOperations);
        }

        public async Task<IEnumerable<FinancialOperationDto>> GetAllByDatePeriodAsync(
            DateOnly startDate,
            DateOnly endDate,
            string accountId,
            CancellationToken cancellationToken = default)
        {
            if (startDate == default)
            {
                throw new ArgumentException("Start date cannot be empty", nameof(startDate));
            }

            if (endDate == default)
            {
                throw new ArgumentException("End date cannot be empty", nameof(endDate));
            }

            if (startDate > endDate)
            {
                throw new ArgumentException("Start date cannot be greater than end date");
            }

            if (string.IsNullOrEmpty(accountId))
            {
                throw new ArgumentException("User Id cannot be empty", nameof(accountId));
            }

            var operations = await _operationRepository.WhereAsync(x =>
                DateOnly.FromDateTime(x.Date) >= startDate && DateOnly.FromDateTime(x.Date) <= endDate, accountId, cancellationToken);

            var sortedOperations = operations.OrderByDescending(x => x.Date);

            return _mapper.Map<IEnumerable<FinancialOperationDto>>(sortedOperations);
        }
    }
}
