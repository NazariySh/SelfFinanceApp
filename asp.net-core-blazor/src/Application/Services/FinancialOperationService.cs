using Application.Interfaces;
using Shared.Dtos;
using Shared.Models;
using System.Net.Http.Json;

namespace Application.Services
{
    public class FinancialOperationService(
        IProtectedHttpClientProvider protectedHttpClientProvider,
        IErrorResponseHandler errorResponseHandler)
        : IFinancialOperationService
    {
        private const string _basePath = "api/financial_operations";
        private readonly IProtectedHttpClientProvider _protectedHttpClientProvider = protectedHttpClientProvider ?? throw new ArgumentNullException(nameof(protectedHttpClientProvider));
        private readonly IErrorResponseHandler _errorResponseHandler = errorResponseHandler ?? throw new ArgumentNullException(nameof(errorResponseHandler));

        #region User panel

        public async Task CreateAsync(FinancialOperationForCreationDto dto, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(dto);

            using var response = await _protectedHttpClientProvider.PostAsJsonAsync(_basePath, dto, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                await _errorResponseHandler.HandleResponseAsync(response, cancellationToken);
            }
        }

        public async Task UpdateAsync(Guid id, FinancialOperationForUpdateDto dto, CancellationToken cancellationToken = default)
        {
            if (id == Guid.Empty)
            {
                throw new ArgumentException("The id cannot be empty.", nameof(id));
            }

            ArgumentNullException.ThrowIfNull(dto);

            using var response = await _protectedHttpClientProvider.PutAsJsonAsync($"{_basePath}/{id}", dto, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                await _errorResponseHandler.HandleResponseAsync(response, cancellationToken);
            }
        }

        public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            if (id == Guid.Empty)
            {
                throw new ArgumentException("The id cannot be empty.", nameof(id));
            }

            using var response = await _protectedHttpClientProvider.DeleteAsync($"{_basePath}/{id}", cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                await _errorResponseHandler.HandleResponseAsync(response, cancellationToken);
            }
        }

        public async Task<PagedList<FinancialOperationDto>> GetAllAsync(PaginationParameters parameters, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(parameters);

            var url = $"{_basePath}?pageNumber={parameters.PageNumber}&pageSize={parameters.PageSize}";

            using var response = await _protectedHttpClientProvider.GetAsync(url, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                await _errorResponseHandler.HandleResponseAsync(response, cancellationToken);
            }

            return await response.Content.ReadFromJsonAsync<PagedList<FinancialOperationDto>>(cancellationToken) ?? new PagedList<FinancialOperationDto>([], parameters.PageNumber, parameters.PageSize, 0);
        }

        public async Task<FinancialOperationDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            if (id == Guid.Empty)
            {
                throw new ArgumentException("The id cannot be empty.", nameof(id));
            }

            using var response = await _protectedHttpClientProvider.GetAsync($"{_basePath}/{id}", cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                await _errorResponseHandler.HandleResponseAsync(response, cancellationToken);
            }

            return await response.Content.ReadFromJsonAsync<FinancialOperationDto>(cancellationToken);
        }

        #endregion

        #region Admin panel

        public async Task UpdateAsync(Guid id, FinancialOperationForUpdateDto dto, string accountId, CancellationToken cancellationToken = default)
        {
            if (id == Guid.Empty)
            {
                throw new ArgumentException("The id cannot be empty.", nameof(id));
            }

            ArgumentNullException.ThrowIfNull(dto);

            if (string.IsNullOrEmpty(accountId))
            {
                throw new ArgumentException("The account id cannot be empty.", nameof(accountId));
            }

            using var response = await _protectedHttpClientProvider.PutAsJsonAsync($"{_basePath}/admin/account/{accountId}/operations/{id}", dto, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                await _errorResponseHandler.HandleResponseAsync(response, cancellationToken);
            }
        }

        public async Task DeleteAsync(Guid id, string accountId, CancellationToken cancellationToken = default)
        {
            if (id == Guid.Empty)
            {
                throw new ArgumentException("The id cannot be empty.", nameof(id));
            }

            if (string.IsNullOrEmpty(accountId))
            {
                throw new ArgumentException("The account id cannot be empty.", nameof(accountId));
            }

            using var response = await _protectedHttpClientProvider.DeleteAsync($"{_basePath}/admin/account/{accountId}/operations/{id}", cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                await _errorResponseHandler.HandleResponseAsync(response, cancellationToken);
            }
        }

        public async Task<PagedList<FinancialOperationDto>> GetAllAsync(string accountId, PaginationParameters parameters, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(accountId))
            {
                throw new ArgumentException("The account id cannot be empty.", nameof(accountId));
            }

            ArgumentNullException.ThrowIfNull(parameters);

            var url = $"{_basePath}/admin/account/{accountId}/operations?pageNumber={parameters.PageNumber}&pageSize={parameters.PageSize}";

            using var response = await _protectedHttpClientProvider.GetAsync(url, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                await _errorResponseHandler.HandleResponseAsync(response, cancellationToken);
            }

            return await response.Content.ReadFromJsonAsync<PagedList<FinancialOperationDto>>(cancellationToken) ?? new PagedList<FinancialOperationDto>([], parameters.PageNumber, parameters.PageSize, 0);
        }

        public async Task<FinancialOperationDto?> GetByIdAsync(Guid id, string accountId, CancellationToken cancellationToken = default)
        {
            if (id == Guid.Empty)
            {
                throw new ArgumentException("The id cannot be empty.", nameof(id));
            }

            if (string.IsNullOrEmpty(accountId))
            {
                throw new ArgumentException("The account id cannot be empty.", nameof(accountId));
            }

            using var response = await _protectedHttpClientProvider.GetAsync($"{_basePath}/admin/account/{accountId}/operations/{id}", cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                await _errorResponseHandler.HandleResponseAsync(response, cancellationToken);
            }

            return await response.Content.ReadFromJsonAsync<FinancialOperationDto>(cancellationToken);
        }

        #endregion
    }
}
