using System.Net.Http.Headers;
using System.Net;
using Application.Interfaces;

namespace Application.Handlers
{
    public class AuthenticationHandler(
        IProtectedStorageService protectedStorageService,
        ICustomAuthStateProvider customAuthStateProvider)
        : IAuthenticationHandler
    {
        private const string _accessTokenAuthKey = "Access_Token";
        private readonly IProtectedStorageService _protectedStorageService = protectedStorageService ?? throw new ArgumentNullException(nameof(protectedStorageService));
        private readonly ICustomAuthStateProvider _customAuthStateProvider = customAuthStateProvider ?? throw new ArgumentNullException(nameof(customAuthStateProvider));
        private readonly SemaphoreSlim _refreshGate = new(1, 1);

        public async Task<HttpResponseMessage> SendAsync(HttpClient httpClient, HttpRequestMessage request, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(httpClient);
            ArgumentNullException.ThrowIfNull(request);

            var accessToken = await GetAccessTokenAsync(cancellationToken);

            SetJwtAuthorizationHeader(httpClient, accessToken);

            var response = await httpClient.SendAsync(request, cancellationToken);

            if (response.StatusCode != HttpStatusCode.Unauthorized || string.IsNullOrEmpty(accessToken))
            {
                return response;
            }

            // Single-flight token refresh; concurrent 401s wait on the same refresh.
            if (!await _refreshGate.WaitAsync(0, cancellationToken))
            {
                await _refreshGate.WaitAsync(cancellationToken);
                _refreshGate.Release();

                return await RetryAsync(httpClient, request, cancellationToken);
            }

            try
            {
                await _customAuthStateProvider.RefreshAuthenticationStateAsync(cancellationToken);
            }
            finally
            {
                _refreshGate.Release();
            }

            response.Dispose();

            return await RetryAsync(httpClient, request, cancellationToken);
        }

        private async Task<HttpResponseMessage> RetryAsync(HttpClient httpClient, HttpRequestMessage originalRequest, CancellationToken cancellationToken)
        {
            var accessToken = await GetAccessTokenAsync(cancellationToken);

            SetJwtAuthorizationHeader(httpClient, accessToken);

            using var retryRequest = await CloneRequestAsync(originalRequest, cancellationToken);

            return await httpClient.SendAsync(retryRequest, cancellationToken);
        }

        private static async Task<HttpRequestMessage> CloneRequestAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var clone = new HttpRequestMessage(request.Method, request.RequestUri)
            {
                Version = request.Version
            };

            foreach (var header in request.Headers)
            {
                clone.Headers.TryAddWithoutValidation(header.Key, header.Value);
            }

            foreach (var prop in request.Options)
            {
                ((IDictionary<string, object?>)clone.Options)[prop.Key] = prop.Value;
            }

            if (request.Content is not null)
            {
                var buffered = await request.Content.ReadAsByteArrayAsync(cancellationToken);
                var content = new ByteArrayContent(buffered);

                foreach (var header in request.Content.Headers)
                {
                    content.Headers.TryAddWithoutValidation(header.Key, header.Value);
                }

                clone.Content = content;
            }

            return clone;
        }

        private static void SetJwtAuthorizationHeader(HttpClient client, string? jwtToken)
        {
            if (!string.IsNullOrEmpty(jwtToken))
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);
            }
        }

        private ValueTask<string?> GetAccessTokenAsync(CancellationToken cancellationToken)
        {
            return _protectedStorageService.GetAsync<string>(_accessTokenAuthKey, cancellationToken);
        }
    }
}
