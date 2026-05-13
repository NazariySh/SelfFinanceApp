namespace Domain.Exceptions
{
    public class OperationAlreadyExistException : AlreadyExistException
    {
        public OperationAlreadyExistException()
        {
        }

        public OperationAlreadyExistException(string message) : base(message)
        {
        }

        public OperationAlreadyExistException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
