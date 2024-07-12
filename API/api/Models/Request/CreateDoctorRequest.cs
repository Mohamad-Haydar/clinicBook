using System.ComponentModel.DataAnnotations.Schema;

namespace api.Models.Request;

public class CreateDoctorRequest
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string password {get; set;}
    public string PhoneNumber { get; set; }
    public string Description { get; set; }
    public int CategoryId { get; set; }

}