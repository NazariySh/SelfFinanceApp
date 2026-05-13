using Application.Interfaces;
using Shared.Dtos;
using Shared.Models;
using System.Net.Http.Json;

namespace Application.Services
{
    public class AccountService(
        IProtectedHttpClientProvider protectedHttpClientProvider,
        IErrorResponseHandler errorResponseHandler)
        : IAccountService
    {
        private const string _basePath = "api/account";
        private readonly IProtectedHttpClientProvider _protectedHttpClientProvider = protectedHttpClientProvider ?? throw new ArgumentNullException(nameof(protectedHttpClientProvider));
        private readonly IErrorResponseHandler _errorResponseHandler = errorResponseHandler ?? throw new ArgumentNullException(nameof(errorResponseHandler));

        #region User panel

        public async Task CreateAsync(AppUserForCreationDto userDto, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(userDto);

            using var response = await _protectedHttpClientProvider.PostAsJsonAsync($"{_basePath}/register", userDto, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                await _errorResponseHandler.HandleResponseAsync(response, cancellationToken);
            }
        }

        public async Task UpdateAsync(AppUserForUpdateDto userDto, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(userDto);

            using var response = await _protectedHttpClientProvider.PutAsJsonAsync(_basePath, userDto, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                await _errorResponseHandler.HandleResponseAsync(response, cancellationToken);
            }
        }

        public async Task DeleteAsync(CancellationToken cancellationToken = default)
        {
            using var response = await _protectedHttpClientProvider.DeleteAsync(_basePath, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                await _errorResponseHandler.HandleResponseAsync(response, cancellationToken);
            }
        }

        public async Task<AppUserDto?> GetAsync(CancellationToken cancellationToken = default)
        {
            using var response = await _protectedHttpClientProvider.GetAsync(_basePath, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                await _errorResponseHandler.HandleResponseAsync(response, cancellationToken);
            }

            return await response.Content.ReadFromJsonAsync<AppUserDto>(cancellationToken);
        }

        #endregion

        #region Admin panel

        public async Task CreateAsync(AppUserForCreationDto userDto, string roleName, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(userDto);

            if (string.IsNullOrEmpty(roleName))
            {
                throw new ArgumentException("Role name cannot be empty.", nameof(roleName));
            }

            using var response = await _protectedHttpClientProvider.PostAsJsonAsync($"{_basePath}/admin/register?roleName={roleName}", userDto, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                await _errorResponseHandler.HandleResponseAsync(response, cancellationToken);
            }
        }

        public async Task UpdateAsync(string id, AppUserForUpdateDto userDto, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentException("The account id cannot be empty.", nameof(id));
            }

            ArgumentNullException.ThrowIfNull(userDto);

            using var response = await _protectedHttpClientProvider.PutAsJsonAsync($"{_basePath}/admin/users/{id}", userDto, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                await _errorResponseHandler.HandleResponseAsync(response, cancellationToken);
            }
        }

        public async Task DeleteAsync(string id, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentException("The account id cannot be empty.", nameof(id));
            }

            using var response = await _protectedHttpClientProvider.DeleteAsync($"{_basePath}/admin/users/{id}", cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                await _errorResponseHandler.HandleResponseAsync(response, cancellationToken);
            }
        }

        public async Task<AppUserDto?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentException("The account id cannot be empty.", nameof(id));
            }

            using var response = await _protectedHttpClientProvider.GetAsync($"{_basePath}/admin/users/{id}", cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                await _errorResponseHandler.HandleResponseAsync(response, cancellationToken);
            }

            return await response.Content.ReadFromJsonAsync<AppUserDto>(cancellationToken);
        }

        public async Task<PagedList<AppUserDto>> GetAllAsync(PaginationParameters parameters, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(parameters);

            var url = $"{_basePath}/admin/users?pageNumber={parameters.PageNumber}&pageSize={parameters.PageSize}";

            using var response = await _protectedHttpClientProvider.GetAsync(url, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                await _errorResponseHandler.HandleResponseAsync(response, cancellationToken);
            }

            return await response.Content.ReadFromJsonAsync<PagedList<AppUserDto>>(cancellationToken) ?? new PagedList<AppUserDto>([], parameters.PageNumber, parameters.PageSize, 0);
        }

        public async Task AddRoleAsync(string id, string roleName, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentException("The account id cannot be empty.", nameof(id));
            }

            if (string.IsNullOrEmpty(roleName))
            {
                throw new ArgumentException("Role name cannot be empty.", nameof(roleName));
            }

            using var response = await _protectedHttpClientProvider.PostAsync($"{_basePath}/admin/users/{id}/roles?roleName={roleName}", null, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                await _errorResponseHandler.HandleResponseAsync(response, cancellationToken);
            }
        }

        public async Task RemoveRoleAsync(string id, string roleName, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentException("The account id cannot be empty.", nameof(id));
            }

            if (string.IsNullOrEmpty(roleName))
            {
                throw new ArgumentException("Role name cannot be empty.", nameof(roleName));
            }

            using var response = await _protectedHttpClientProvider.DeleteAsync($"{_basePath}/admin/users/{id}/roles?roleName={roleName}", cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                await _errorResponseHandler.HandleResponseAsync(response, cancellationToken);
            }
        }

        #endregion
    }
}
