namespace Application.Interfaces
{
    public interface IProtectedStorageService
    {
        ValueTask<TValue?> GetAsync<TValue>(string key, CancellationToken cancellationToken = default);
        ValueTask RemoveAsync(string key, CancellationToken cancellationToken = default);
        ValueTask SetAsync<TValue>(string key, TValue value, CancellationToken cancellationToken = default);
    }
}
