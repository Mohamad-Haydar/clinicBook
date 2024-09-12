using System.ComponentModel.DataAnnotations.Schema;

namespace api.Models;

[Table("doctorservice")]
public class DoctorServiceModel
{
    [Column("id")]
    public int Id { get; set; }
    [Column("duration")]
    public int Duration { get; set; }
    [Column("doctorid")]
    public string DoctorId { get; set; }
    [Column("serviceid")]
    public int ServiceId { get; set; }
    public ServiceModel services { get; set; }
}