using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace api.Models;

[Table("doctoravailability")]
public class DoctorAvailabilityModel
{
    [Column("id")]
    public int Id { get; set; }
    [Column("availabledate")]
    public DateOnly AvailableDate { get; set; }
    [Column("dayname")]
    public string DayName { get; set; }
    [Column("starthour")]
    public DateTimeOffset StartHour { get; set; }
    [Column("endhour")]
    public DateTimeOffset EndHour { get; set; }
    [Column("maxclient")]
    public int MaxClient { get; set; }
    [Column("currentreservations")]
    public int CurrentReservations { get; set; }
    [Column("doctorid")]
    public string DoctorId { get; set; }
}