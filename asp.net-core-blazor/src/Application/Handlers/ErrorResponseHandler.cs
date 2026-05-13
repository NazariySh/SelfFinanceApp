using Domain.Exceptions;
using Application.Interfaces;
using Shared.Models;
using System.Net;
using System.Text.Json;

namespace Application.Handlers
{
    public class ErrorResponseHandler : IErrorResponseHandler
    {
        public async Task HandleResponseAsync(HttpResponseMessage response, CancellationToken cancellationToken = default)
        {
            var content = await response.Content.ReadAsStringAsync(cancellationToken);

            if (string.IsNullOrEmpty(content))
            {
                throw new InvalidOperationException("Failed to read the response content.");
            }

            if (response.StatusCode == HttpStatusCode.InternalServerError)
            {
                var error = JsonSerializer.Deserialize<ErrorResponse>(content) ?? throw new InvalidOperationException("Failed to deserialize the server error response.");

                throw new InvalidOperationException(error.Message);
            }
            else if (response.StatusCode == HttpStatusCode.UnprocessableEntity)
            {
                var validationError = JsonSerializer.Deserialize<ValidationErrorResponse>(content) ?? throw new InvalidOperationException("Failed to deserialize the validation error response.");

                throw new ValidationException(validationError.Errors);
            }
            else
            {
                var error = JsonSerializer.Deserialize<ErrorResponse>(content) ?? throw new InvalidOperationException("Failed to deserialize the error response.");

                throw new HttpRequestException(error.Message);
            }
        }
    }
}
