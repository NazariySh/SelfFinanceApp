namespace Application.Interfaces
{
    public interface IAuthenticationHandler
    {
        Task<HttpResponseMessage> SendAsync(HttpClient httpClient, HttpRequestMessage request, CancellationToken cancellationToken);
    }
}
