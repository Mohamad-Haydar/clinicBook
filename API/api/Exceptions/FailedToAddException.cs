namespace api.Exceptions;

public class FailedToAddException : Exception
{
    public FailedToAddException(string message = "حدث خطأ اثناء الاضافة, الرجاء المحاولة مجددا") : base(message) { }    
}