using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace api.Models.Request;

public class CreateDoctorRequest : CreateUserRequest
{
    [Required(ErrorMessage = "Description is required")]
    public string Description { get; set; }
    
    [Required(ErrorMessage = "CategoryId is required")]
    [Range(1, int.MaxValue, ErrorMessage = "Number of people must be greater than 0")]
    public int CategoryId { get; set; }
}