using System.ComponentModel.DataAnnotations.Schema;

namespace api.Models;

[Table("client")]
public class ClientModel
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
}