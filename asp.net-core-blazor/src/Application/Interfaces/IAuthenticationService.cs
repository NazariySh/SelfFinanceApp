using Shared.Dtos;

namespace Application.Interfaces
{
    public interface IAuthenticationService
    {
        Task LoginAsync(LoginDto loginDto, CancellationToken cancellationToken = default);
        Task LogoutAsync(CancellationToken cancellationToken = default);
    }
}
