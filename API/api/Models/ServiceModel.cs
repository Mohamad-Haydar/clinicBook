using System.ComponentModel.DataAnnotations.Schema;

namespace api.Models;

[Table("service")]
public class ServiceModel
{
    [Column("id")]
    public int Id { get; set; }
    [Column("servicename")]
    public string ServiceName { get; set; }
}