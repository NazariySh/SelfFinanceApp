using Domain.Models;
using System.Security.Claims;

namespace Application.Interfaces
{
    public interface IJwtTokenService
    {
        public Token CreateToken(IEnumerable<Claim> claims);
        string GenerateAccessToken(IEnumerable<Claim> claims);
        RefreshToken GenerateRefreshToken();
        ClaimsPrincipal GetPrincipalFromAccessToken(string accessToken);
    }
}
