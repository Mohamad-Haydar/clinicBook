namespace api.Exceptions;

public class InvalidRequestException : Exception
{
    public InvalidRequestException(string message = "الرجاء التأكد من المعلومات قبل الارسال") : base(message){ }
}