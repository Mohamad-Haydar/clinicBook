namespace api.Models.Responce;

public class ErrorResponse(string message = "An error occurred while processing your request.")
{
    public string Message { get; set; } = message;
}