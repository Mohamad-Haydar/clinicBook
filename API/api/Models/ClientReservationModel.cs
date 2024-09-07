using System.ComponentModel.DataAnnotations.Schema;

namespace api.Models;

[Table("clientreservation")]
public class ClientReservationModel
{
    [Column("id")]
    public int Id { get; set; }
    [Column("starttime")]
    public TimeSpan StartTime { get; set; }
    [Column("endtime")]
    public TimeSpan EndTime { get; set; }
    [Column("doctoravailabilityid")]
    public int DoctorAvailabilityId { get; set; }
    [Column("isdone")]
    public bool IsDone { get; set; }
    [Column("clientid")]
    public string ClientId { get; set; }

    [Column("details")]
    public string? Details { get; set; }
}