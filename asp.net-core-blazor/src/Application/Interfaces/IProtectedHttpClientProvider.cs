namespace Application.Interfaces
{
    public interface IProtectedHttpClientProvider
    {
        Task<HttpResponseMessage> GetAsync(string requestUri, CancellationToken cancellationToken = default);
        Task<HttpResponseMessage> PostAsync(string requestUri, HttpContent? content, CancellationToken cancellationToken = default);
        Task<HttpResponseMessage> PostAsJsonAsync<TValue>(string requestUri, TValue value, CancellationToken cancellationToken = default);
        Task<HttpResponseMessage> PutAsJsonAsync<TValue>(string requestUri, TValue value, CancellationToken cancellationToken = default);
        Task<HttpResponseMessage> DeleteAsync(string requestUri, CancellationToken cancellationToken = default);
        Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken = default);
    }
}