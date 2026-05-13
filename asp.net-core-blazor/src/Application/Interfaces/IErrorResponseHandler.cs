namespace Application.Interfaces
{
    public interface IErrorResponseHandler
    {
        Task HandleResponseAsync(HttpResponseMessage response, CancellationToken cancellationToken = default);
    }
}
