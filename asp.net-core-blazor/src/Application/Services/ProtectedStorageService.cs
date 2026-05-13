using Application.Interfaces;
using Blazored.SessionStorage;
using Microsoft.AspNetCore.DataProtection;
using System.Text.Json;

namespace Application.Services
{
    public class ProtectedStorageService(
        ISessionStorageService sessionStorageService,
        IDataProtectionProvider dataProtectionProvider)
        : IProtectedStorageService
    {
        private readonly ISessionStorageService _sessionStorageService = sessionStorageService ?? throw new ArgumentNullException(nameof(sessionStorageService));
        private readonly IDataProtector _dataProtector = dataProtectionProvider.CreateProtector(nameof(ProtectedStorageService));

        public async ValueTask<TValue?> GetAsync<TValue>(string key, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(key);

            var protectedData = await _sessionStorageService.GetItemAsync<string>(key, cancellationToken);

            if (protectedData is null)
            {
                return default;
            }

            var jsonData = _dataProtector.Unprotect(protectedData);

            return JsonSerializer.Deserialize<TValue>(jsonData);
        }

        public ValueTask RemoveAsync(string key, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(key);

            return _sessionStorageService.RemoveItemAsync(key, cancellationToken);
        }

        public ValueTask SetAsync<TValue>(string key, TValue value, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(key);

            ArgumentNullException.ThrowIfNull(value);

            var jsonData = JsonSerializer.Serialize(value);

            var protectedData = _dataProtector.Protect(jsonData);

            return _sessionStorageService.SetItemAsync(key, protectedData, cancellationToken);
        }
    }
}
