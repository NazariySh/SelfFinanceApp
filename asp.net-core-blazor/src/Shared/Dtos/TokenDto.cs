using Shared.Models;

namespace Shared.Dtos
{
    public record TokenDto(string AccessToken, RefreshToken RefreshToken);
}
