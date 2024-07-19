using System.ComponentModel.DataAnnotations;

namespace api.Models.Request
{
    public class CreateUserRequest
    {
        [Required(ErrorMessage = "FirstName is required")]
        [StringLength(20, MinimumLength = 2, ErrorMessage = "First Name must be between 2 and 20 characters long")]
        public string FirstName { get; set; }
        
        [Required(ErrorMessage = "LastName is required")]
        [StringLength(20, MinimumLength = 2, ErrorMessage = "Last Name must be between 2 and 20 characters long")]
        public string LastName { get; set; }
        
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "email address must be between 2 and 50 characters long")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Confirm Password is required")]
        [Compare("Password", ErrorMessage = "Passwords do not match")]
        public string ConfirmPassword { get; set; }

        
        [Required(ErrorMessage = "PhoneNumber is required")]
        [Phone(ErrorMessage = "Invalid phone number")]
        public string PhoneNumber { get; set; }
    }
}

