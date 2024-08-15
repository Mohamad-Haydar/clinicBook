namespace api.Models.Responce
{
    public class BadRequestResponse(string message = "الرجاء التأكد من المعلومات قبل الارسال")
    {
        public string Message { get; set; } = message;
    }
}
