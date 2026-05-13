namespace Domain.Exceptions
{
    public class ValidationException : Exception
    {
        public Dictionary<string, string> Errors { get; } = [];

        public ValidationException()
        {
        }

        public ValidationException(Dictionary<string, string> errors) : this("One or more validation failures have occurred.", errors)
        {
        }

        public ValidationException(string? message, Dictionary<string, string> errors) : base(message)
        {
            Errors = errors;
        }

        public ValidationException(string? message) : base(message)
        {
        }

        public ValidationException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}
