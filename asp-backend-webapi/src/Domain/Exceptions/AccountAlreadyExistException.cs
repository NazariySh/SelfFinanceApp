namespace Domain.Exceptions
{
    public class AccountAlreadyExistException : AlreadyExistException
    {
        public AccountAlreadyExistException()
        {
        }

        public AccountAlreadyExistException(string message) : base(message)
        {
        }

        public AccountAlreadyExistException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
