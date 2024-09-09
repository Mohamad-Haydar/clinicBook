using System.ComponentModel.DataAnnotations.Schema;

namespace api.Models;

[Table("category")]
public class CategoryModel
{
    [Column("id")]
    public int Id { get; set; }
    [Column("categoryname")]
    public string CategoryName { get; set; }

    // Optional: Navigation property for related doctors
    public ICollection<DoctorModel> Doctors { get; set; }
}