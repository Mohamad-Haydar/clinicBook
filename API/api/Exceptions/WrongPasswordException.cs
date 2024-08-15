namespace api.Exceptions;

public class WrongPasswordException : Exception
{
    public WrongPasswordException(string message="الرقم السري خاطء, الرجاء المحاولة مجددا") : base(message){ }
}