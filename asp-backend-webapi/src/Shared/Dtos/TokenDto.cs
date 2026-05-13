namespace Shared.Dtos
{
    public class TokenDto
    {
        public string AccessToken { get; set; } = string.Empty;
        public RefreshTokenDto RefreshToken { get; set; } = new();
    }
}
