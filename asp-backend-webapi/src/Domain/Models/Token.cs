namespace Domain.Models
{
    public class Token
    {
        public string AccessToken { get; set; } = string.Empty;
        public RefreshToken? RefreshToken { get; set; }
    }
}
