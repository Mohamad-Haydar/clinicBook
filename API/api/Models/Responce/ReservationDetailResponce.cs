namespace api.Models.Responce;

public class ReservationDetailResponce
{
    #pragma warning disable IDE1006, CS8618 // Naming Styles
    public int id { get; set; }
    public TimeSpan startTime { get; set; }
    public TimeSpan endTime { get; set; }
    public string[] serviceNames { get; set; }
    public string clientName { get; set; }
    public bool isDone { get; set; }
    public string? details { get; set; }
#pragma warning restore IDE1006, CS8618 // Naming Styles
}