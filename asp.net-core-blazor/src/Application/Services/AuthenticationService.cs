using Application.Interfaces;
using Shared.Dtos;
using System.Net.Http.Json;

namespace Application.Services
{
    public class AuthenticationService(
        IProtectedHttpClientProvider protectedHttpClientProvider,
        ICustomAuthStateProvider customAuthStateProvider,
        IErrorResponseHandler errorResponseHandler)
        : IAuthenticationService
    {
        private const string _basePath = "api/account/auth";
        private readonly IProtectedHttpClientProvider _protectedHttpClientProvider = protectedHttpClientProvider ?? throw new ArgumentNullException(nameof(protectedHttpClientProvider));
        private readonly ICustomAuthStateProvider _customAuthStateProvider = customAuthStateProvider ?? throw new ArgumentNullException(nameof(customAuthStateProvider));
        private readonly IErrorResponseHandler _errorResponseHandler = errorResponseHandler ?? throw new ArgumentNullException(nameof(errorResponseHandler));

        public async Task LoginAsync(LoginDto loginDto, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(loginDto);

            using var response = await _protectedHttpClientProvider.PostAsJsonAsync($"{_basePath}/login", loginDto, cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                var token = await response.Content.ReadFromJsonAsync<TokenDto>(cancellationToken);

                if (token != null)
                {
                    await _customAuthStateProvider.UpdateAuthenticationStateAsync(token, cancellationToken);
                }
            }
            else
            {
                await _errorResponseHandler.HandleResponseAsync(response, cancellationToken);
            }
        }

        public async Task LogoutAsync(CancellationToken cancellationToken = default)
        {
            await _customAuthStateProvider.ClearAuthenticationStateAsync(cancellationToken);

            await _protectedHttpClientProvider.DeleteAsync($"{_basePath}/logout", cancellationToken);
        }
    }
}
