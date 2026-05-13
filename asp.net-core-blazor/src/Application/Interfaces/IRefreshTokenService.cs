using Shared.Dtos;

namespace Application.Interfaces
{
    public interface IRefreshTokenService
    {
        Task<TokenDto> RefreshTokenAsync(TokenDto tokenDto, CancellationToken cancellationToken = default);
    }
}
