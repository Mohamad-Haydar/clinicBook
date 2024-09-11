using System.ComponentModel.DataAnnotations;

namespace api.Models.Request
{
    public class UpdateUserRequest
    {
        [Required(ErrorMessage = "FirstName is required")]
        [StringLength(20, MinimumLength = 2, ErrorMessage = "First Name must be between 2 and 20 characters long")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "email address must be between 2 and 50 characters long")]
        public string Email { get; set; }

        [Required(ErrorMessage = "PhoneNumber is required")]
        [Phone(ErrorMessage = "Invalid phone number")]
        public string PhoneNumber { get; set; }
    }
}
