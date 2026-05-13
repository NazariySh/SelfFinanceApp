namespace Shared.Models
{
    public record ErrorResponse(bool Status, int StatusCode, string Message);
}
