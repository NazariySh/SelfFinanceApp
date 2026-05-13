using Application.Interfaces;
using Domain.Models;
using Microsoft.Extensions.Options;
using Shared.Dtos;
using System.Net.Http.Json;

namespace Application.Services
{
    public class RefreshTokenService(
        IOptions<WebApiSettings> webApiSettings,
        IHttpClientFactory httpClientFactory)
        : IRefreshTokenService
    {
        private readonly WebApiSettings _webApiSettings = webApiSettings.Value ?? throw new ArgumentNullException(nameof(webApiSettings));
        private readonly IHttpClientFactory _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));

        public async Task<TokenDto> RefreshTokenAsync(TokenDto tokenDto, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(tokenDto);

            using var client = _httpClientFactory.CreateClient(_webApiSettings.Name);

            using var response = await client.PostAsJsonAsync("api/account/auth/refresh-token", tokenDto, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                throw new UnauthorizedAccessException("Failed to refresh the access token.");
            }

            var newToken = await response.Content.ReadFromJsonAsync<TokenDto>(cancellationToken);

            if (newToken is null || string.IsNullOrEmpty(newToken.AccessToken) || string.IsNullOrEmpty(newToken.RefreshToken?.Token))
            {
                throw new UnauthorizedAccessException("Refresh token endpoint returned an invalid token.");
            }

            return newToken;
        }
    }
}
