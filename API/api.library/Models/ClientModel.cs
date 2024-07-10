using System.ComponentModel.DataAnnotations.Schema;

namespace api.library.Models;

[Table("client")]
public class ClientModel
{

    [Column("id")]
    public int Id { get; set; }
    [Column("first_name")]
    public string FirstName { get; set; }
    [Column("last_name")]
    public string LastName { get; set; }
    [Column("email")]
    public string Email { get; set; }
    [Column("phone_number")]
    public string PhoneNumber { get; set; }
}