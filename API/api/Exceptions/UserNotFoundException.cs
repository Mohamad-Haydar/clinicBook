namespace api.Exceptions
{
    public class UserUserNotFoundException : Exception
    {
        public UserUserNotFoundException(string message = "البريد الالكتروني غير موجود, الرجاء التأكد من البريد") : base(message) { }
    }
}
