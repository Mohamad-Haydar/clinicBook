using System.ComponentModel.DataAnnotations.Schema;

namespace api.library.Models;

[Table("doctorservice")]
public class DoctorServiceModel
{
    [Column("id")]
    public int Id { get; set; }
    [Column("servicename")]
    public string ServiceName { get; set; }
    [Column("duration")]
    public int Duration { get; set; }
    [Column("doctorid")]
    public string DoctorId { get; set; }
    [Column("serviceid")]
    public int ServiceId { get; set; }
}