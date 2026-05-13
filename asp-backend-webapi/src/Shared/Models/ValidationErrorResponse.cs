namespace Shared.Models
{
    public record ValidationErrorResponse(bool Status, int StatusCode, Dictionary<string, string> Errors);
}
