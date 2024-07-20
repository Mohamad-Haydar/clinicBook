namespace api.Exceptions;

public class FailedToAddException : Exception
{
    public FailedToAddException(string message) : base(message) { }    
}