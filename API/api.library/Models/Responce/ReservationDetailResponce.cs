namespace api.library.Models.Responce;

public class ReservationDetailResponce
{
    #pragma warning disable IDE1006, CS8618 // Naming Styles
    public int id { get; set; }
    public DateTimeOffset start_time { get; set; }
    public DateTimeOffset end_time { get; set; }
    public int doctor_availability_id { get; set; }
    public string[] service_names { get; set; }
    public string doctor_id { get; set; }
    #pragma warning restore IDE1006, CS8618 // Naming Styles
}