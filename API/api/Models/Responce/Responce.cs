namespace api.Models.Responce;

public class Responce(string message = "An error occurred while processing your request.")
{
    public string Message { get; set; } = message;
}