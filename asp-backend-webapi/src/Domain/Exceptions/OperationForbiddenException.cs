namespace Domain.Exceptions
{
    public class OperationForbiddenException : ForbiddenException
    {
        public OperationForbiddenException()
        {
        }

        public OperationForbiddenException(string message) : base(message)
        {
        }

        public OperationForbiddenException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
