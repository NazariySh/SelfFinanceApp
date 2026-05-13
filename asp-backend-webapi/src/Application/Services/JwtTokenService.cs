using Application.Interfaces;
using Microsoft.IdentityModel.Tokens;
using Domain.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;

namespace Application.Services
{
    public class JwtTokenService(IOptions<JwtSettings> jwtSettings) : IJwtTokenService
    {
        private const string _algorithm = SecurityAlgorithms.HmacSha256;
        private readonly JwtSecurityTokenHandler _jwtSecurityTokenHandler = new();
        private readonly JwtSettings _jwtSettings = jwtSettings.Value ?? throw new ArgumentNullException(nameof(jwtSettings));

        public Token CreateToken(IEnumerable<Claim> claims)
        {
            ArgumentNullException.ThrowIfNull(claims);

            return new Token
            {
                AccessToken = GenerateAccessToken(claims),
                RefreshToken = GenerateRefreshToken()
            };
        }

        public string GenerateAccessToken(IEnumerable<Claim> claims)
        {
            ArgumentNullException.ThrowIfNull(claims);

            var token = new JwtSecurityToken(
                    issuer: _jwtSettings.Issuer,
                    audience: _jwtSettings.Audience,
                    claims: claims,
                    expires: DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenExpirationInMinutes),
                    signingCredentials: new SigningCredentials(GetSecureKey(), _algorithm)
                );

            return _jwtSecurityTokenHandler.WriteToken(token);
        }

        public RefreshToken GenerateRefreshToken()
        {
            var token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));

            return new RefreshToken
            {
                Token = token,
                ExpiryTime = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpirationInDays)
            };
        }

        public ClaimsPrincipal GetPrincipalFromAccessToken(string accessToken)
        {
            if (string.IsNullOrEmpty(accessToken))
            {
                throw new ArgumentException("Access token cannot be empty", nameof(accessToken));
            }

            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = false,
                ValidateIssuerSigningKey = true,
                ValidIssuer = _jwtSettings.Issuer,
                ValidAudience = _jwtSettings.Audience,
                IssuerSigningKey = GetSecureKey(),
                ClockSkew = TimeSpan.Zero
            };

            var principal = _jwtSecurityTokenHandler.ValidateToken(accessToken, tokenValidationParameters, out var securityToken);

            if (securityToken is not JwtSecurityToken jwtSecurityToken ||
                !jwtSecurityToken.Header.Alg.Equals(_algorithm, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new SecurityTokenException("Invalid token");
            }

            return principal;
        }

        private SymmetricSecurityKey GetSecureKey()
        {
            if (string.IsNullOrEmpty(_jwtSettings.Key))
            {
                throw new InvalidOperationException("JWT key is not set");
            }

            return new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
        }
    }
}
