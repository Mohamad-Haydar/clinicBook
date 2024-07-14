using System.ComponentModel.DataAnnotations.Schema;

namespace api.Models;

[Table("clientreservation")]
public class ClientReservation
{
    [Column("id")]
    public int Id { get; set; }
    [Column("starttime")]
    public DateTimeOffset StartTime { get; set; }
    [Column("endtime")]
    public DateTimeOffset EndTime { get; set; }
    [Column("doctoravailabilityid")]
    public int DoctorAvailabilityId { get; set; }
    [Column("clientid")]
    public string ClientId { get; set; }
}