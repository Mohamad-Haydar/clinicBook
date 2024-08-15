namespace api.Exceptions
{
    public class UserNotFoundException : Exception
    {
        public UserNotFoundException(string message = "البريد الالكتروني غير موجود, الرجاء التأكد من البريد") : base(message) { }
    }
}
