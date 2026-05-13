namespace Domain.Models
{
    public class RefreshToken
    {
        public string Token { get; set; } = string.Empty;
        public DateTime ExpiryTime { get; set; }
    }
}
