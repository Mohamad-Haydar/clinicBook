using System.ComponentModel.DataAnnotations.Schema;

namespace api.Models;

[Table("doctor")]
public class DoctorModel
{
    [Column("id")]
    public string Id { get; set; }
    [Column("firstname")]
    public string FirstName { get; set; }
    [Column("lastname")]
    public string LastName { get; set; }
    [Column("email")]
    public string Email { get; set; }
    [Column("phonenumber")]
    public string PhoneNumber { get; set; }
    [Column("image")]
    public string? Image { get; set; }
    [Column("description")]
    public string Description { get; set; }
    [Column("categoryid")]
    public int CategoryId { get; set; }

    // Navigation property to Category
    public CategoryModel Category { get; set; }
}