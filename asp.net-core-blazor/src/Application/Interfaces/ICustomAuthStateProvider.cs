using Microsoft.AspNetCore.Components.Authorization;
using Shared.Dtos;

namespace Application.Interfaces
{
    public interface ICustomAuthStateProvider
    {
        Task<AuthenticationState> GetAuthenticationStateAsync();
        Task UpdateAuthenticationStateAsync(TokenDto tokenDto, CancellationToken cancellationToken = default);
        Task ClearAuthenticationStateAsync(CancellationToken cancellationToken = default);
        Task RefreshAuthenticationStateAsync(CancellationToken cancellationToken = default);
    }
}
