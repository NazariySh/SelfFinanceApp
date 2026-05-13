using Domain.Exceptions;
using FluentValidation;
using Shared.Models;
using System.Text.Json;

namespace WebApi.Middleware
{
    public class ExceptionHandlingMiddleware(ILogger<ExceptionHandlingMiddleware> logger) : IMiddleware
    {
        private readonly ILogger<ExceptionHandlingMiddleware> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex, "An error occurred: {errorMessage}", ex.Message);

                await HandleExceptionAsync(context, ex);
            }
        }

        private static async Task HandleExceptionAsync(HttpContext httpContext, Exception exception)
        {
            httpContext.Response.ContentType = "application/json";

            var statusCode = exception switch
            {
                ArgumentNullException or ArgumentException => StatusCodes.Status400BadRequest,
                UnauthorizedAccessException => StatusCodes.Status401Unauthorized,
                ForbiddenException => StatusCodes.Status403Forbidden,
                NotFoundException => StatusCodes.Status404NotFound,
                AlreadyExistException => StatusCodes.Status409Conflict,
                ValidationException => StatusCodes.Status422UnprocessableEntity,
                _ => StatusCodes.Status500InternalServerError
            };

            httpContext.Response.StatusCode = statusCode;

            string? result;

            if (exception is ValidationException validationException)
            {
                var errors = validationException.Errors.ToDictionary(e => e.PropertyName, e => e.ErrorMessage);

                result = JsonSerializer.Serialize(new ValidationErrorResponse(false, statusCode, errors));
            }
            else
            {
                result = JsonSerializer.Serialize(new ErrorResponse(false, statusCode, exception.Message));
            }

            await httpContext.Response.WriteAsync(result);
        }
    }
}
