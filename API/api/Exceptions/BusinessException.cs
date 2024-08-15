namespace api.Exceptions;

public class BusinessException : Exception
{
    public BusinessException(string message = "حدث خطأ, الرجاء المحاولة مجددا") : base(message) { }
}