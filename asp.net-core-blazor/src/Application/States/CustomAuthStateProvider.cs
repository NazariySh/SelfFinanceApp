using Application.Interfaces;
using Microsoft.AspNetCore.Components.Authorization;
using Shared.Dtos;
using Shared.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Application.States
{
    public class CustomAuthStateProvider(
        IProtectedStorageService protectedStorageService,
        IRefreshTokenService refreshTokenService)
        : AuthenticationStateProvider, ICustomAuthStateProvider
    {
        private const string _accessTokenAuthKey = "Access_Token";
        private const string _refreshTokenAuthKey = "Refresh_Token";
        private readonly IProtectedStorageService _protectedStorageService = protectedStorageService ?? throw new ArgumentNullException(nameof(protectedStorageService));
        private readonly IRefreshTokenService _refreshTokenService = refreshTokenService ?? throw new ArgumentNullException(nameof(refreshTokenService));
        private readonly JwtSecurityTokenHandler _jwtSecurityTokenHandler = new();
        private readonly ClaimsPrincipal _anonymous = new(new ClaimsIdentity());

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            try
            {
                var jwtToken = await _protectedStorageService.GetAsync<string>(_accessTokenAuthKey);

                if (string.IsNullOrEmpty(jwtToken))
                {
                    throw new InvalidOperationException("JWT token is missing.");
                }

                var claimsPrincipal = GetClaimsPrincipal(jwtToken);

                return new AuthenticationState(claimsPrincipal);
            }
            catch
            {
                return new AuthenticationState(_anonymous);
            }
        }

        public async Task UpdateAuthenticationStateAsync(TokenDto tokenDto, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(tokenDto);

            await _protectedStorageService.SetAsync(_accessTokenAuthKey, tokenDto.AccessToken, cancellationToken);
            await _protectedStorageService.SetAsync(_refreshTokenAuthKey, tokenDto.RefreshToken, cancellationToken);

            var claimsPrincipal = GetClaimsPrincipal(tokenDto.AccessToken);

            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(claimsPrincipal)));
        }

        public async Task ClearAuthenticationStateAsync(CancellationToken cancellationToken = default)
        {
            await _protectedStorageService.RemoveAsync(_accessTokenAuthKey, cancellationToken);
            await _protectedStorageService.RemoveAsync(_refreshTokenAuthKey, cancellationToken);

            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(_anonymous)));
        }

        public async Task RefreshAuthenticationStateAsync(CancellationToken cancellationToken = default)
        {
            var tokenDto = await GetTokenAsync(cancellationToken);

            var newToken = await _refreshTokenService.RefreshTokenAsync(tokenDto, cancellationToken);

            await UpdateAuthenticationStateAsync(newToken, cancellationToken);
        }

        private async Task<TokenDto> GetTokenAsync(CancellationToken cancellationToken)
        {
            var accessToken = await _protectedStorageService.GetAsync<string>(_accessTokenAuthKey, cancellationToken);
            var refreshToken = await _protectedStorageService.GetAsync<string>(_refreshTokenAuthKey, cancellationToken);

            if (string.IsNullOrEmpty(accessToken) || string.IsNullOrEmpty(refreshToken))
            {
                throw new UnauthorizedAccessException("Access token or refresh token is missing.");
            }

            return new TokenDto(accessToken, new RefreshToken(refreshToken, default));
        }

        private ClaimsPrincipal GetClaimsPrincipal(string jwtToken)
        {
            var claims = GetClaims(jwtToken);

            return !claims.Any()
                ? new ClaimsPrincipal(new ClaimsIdentity())
                : new ClaimsPrincipal(new ClaimsIdentity(claims, "JwtAuth"));
        }

        private IEnumerable<Claim> GetClaims(string jwtToken)
        {
            var token = _jwtSecurityTokenHandler.ReadJwtToken(jwtToken);

            return token.Claims.Where(c =>
                    c.Type == ClaimTypes.NameIdentifier ||
                    c.Type == ClaimTypes.Name ||
                    c.Type == ClaimTypes.Email ||
                    c.Type == ClaimTypes.Role);
        }
    }
}
