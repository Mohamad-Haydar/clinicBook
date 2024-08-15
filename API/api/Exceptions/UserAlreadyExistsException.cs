namespace api.Exceptions;

public class UserAlreadyExistsException : Exception
{
    public UserAlreadyExistsException(string message = " هذا الحساب موجود مسبقا, الرجاء ادخال بريد جديد او تسجيل الدخول الى القديم.") : base(message) { }
}