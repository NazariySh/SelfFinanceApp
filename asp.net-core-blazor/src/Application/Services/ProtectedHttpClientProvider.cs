using Application.Interfaces;
using Domain.Models;
using Microsoft.Extensions.Options;
using System.Net.Http.Json;

namespace Application.Services
{
    public class ProtectedHttpClientProvider(
        IOptions<WebApiSettings> webApiSettings,
        IHttpClientFactory httpClientFactory,
        IAuthenticationHandler authenticationHandler)
        : IProtectedHttpClientProvider
    {
        private readonly WebApiSettings _webApiSettings = webApiSettings.Value ?? throw new ArgumentNullException(nameof(webApiSettings));
        private readonly IHttpClientFactory _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
        private readonly IAuthenticationHandler _authenticationHandler = authenticationHandler ?? throw new ArgumentNullException(nameof(authenticationHandler));

        public async Task<HttpResponseMessage> GetAsync(string requestUri, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(requestUri);

            using var request = new HttpRequestMessage(HttpMethod.Get, requestUri);

            return await SendAsync(request, cancellationToken);
        }

        public async Task<HttpResponseMessage> PostAsync(string requestUri, HttpContent? content, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(requestUri);

            using var request = new HttpRequestMessage(HttpMethod.Post, requestUri)
            {
                Content = content
            };

            return await SendAsync(request, cancellationToken);
        }

        public async Task<HttpResponseMessage> PostAsJsonAsync<TValue>(string requestUri, TValue value, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(requestUri);

            ArgumentNullException.ThrowIfNull(value);

            using var request = new HttpRequestMessage(HttpMethod.Post, requestUri)
            {
                Content = JsonContent.Create(value)
            };

            return await SendAsync(request, cancellationToken);
        }

        public async Task<HttpResponseMessage> PutAsJsonAsync<TValue>(string requestUri, TValue value, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(requestUri);

            ArgumentNullException.ThrowIfNull(value);

            using var request = new HttpRequestMessage(HttpMethod.Put, requestUri)
            {
                Content = JsonContent.Create(value)
            };

            return await SendAsync(request, cancellationToken);
        }

        public async Task<HttpResponseMessage> DeleteAsync(string requestUri, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(requestUri);

            using var request = new HttpRequestMessage(HttpMethod.Delete, requestUri);

            return await SendAsync(request, cancellationToken);
        }

        public async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(request);

            using var client = _httpClientFactory.CreateClient(_webApiSettings.Name);

            return await _authenticationHandler.SendAsync(client, request, cancellationToken);
        }
    }
}
