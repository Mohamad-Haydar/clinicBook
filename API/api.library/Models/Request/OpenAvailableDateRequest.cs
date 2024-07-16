using System.ComponentModel;
using System.Globalization;

namespace api.library.Models.Request;

public class OpenAvailableDateRequest
{
    public int Id { get; set; }
    public DateOnly AvailableDate { get; set; }
    public TimeSpan StartHour { get; set; }
    public TimeSpan EndHour { get; set; }
    public int MaxClient { get; set; }
    public string DoctorId { get; set; }
}