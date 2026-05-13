namespace Domain.Exceptions
{
    public class UserAlreadyHasRoleException : AlreadyExistException
    {
        public UserAlreadyHasRoleException()
        {
        }

        public UserAlreadyHasRoleException(string message) : base(message)
        {
        }

        public UserAlreadyHasRoleException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
