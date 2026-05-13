namespace Application.Interfaces
{
    public interface IValidatorService
    {
        Task ValidateAndThrowAsync<T>(T dto, CancellationToken cancellationToken = default) where T : class;
    }
}
