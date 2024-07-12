using System.ComponentModel;

namespace api.library.Models.Request;

public class OpenAvailableDateRequest
{
    public int Id { get; set; }
    public DateOnly AvailableDate { get; set; }
    public DateTimeOffset StartHour { get; set; }
    public DateTimeOffset EndHour { get; set; }
    public int MaxClient { get; set; }
    public string DoctorId { get; set; }
}