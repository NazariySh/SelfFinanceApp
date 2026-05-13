namespace Shared.Dtos
{
    public class RefreshTokenDto
    {
        public string Token { get; set; } = string.Empty;
        public DateTime ExpiryTime { get; set; }
    }
}
