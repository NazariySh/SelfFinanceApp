namespace Domain.Exceptions
{
    public class OperationNotFoundException : NotFoundException
    {
        public OperationNotFoundException()
        {
        }

        public OperationNotFoundException(string message) : base(message)
        {
        }

        public OperationNotFoundException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
