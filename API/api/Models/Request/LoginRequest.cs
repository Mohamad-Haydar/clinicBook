using System.ComponentModel.DataAnnotations;

namespace api.Models.Request
{
    public class LoginRequest
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "email address must be between 2 and 50 characters long")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }
    }
}
