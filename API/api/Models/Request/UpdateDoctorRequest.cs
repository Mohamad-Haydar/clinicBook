using System.ComponentModel.DataAnnotations;

namespace api.Models.Request
{
    public class UpdateDoctorRequest
    {
        [StringLength(20, MinimumLength = 2, ErrorMessage = "First Name must be between 2 and 20 characters long")]
        public string? FirstName { get; set; }

        [StringLength(20, MinimumLength = 2, ErrorMessage = "Last Name must be between 2 and 20 characters long")]
        public string? LastName { get; set; }

        [EmailAddress(ErrorMessage = "Invalid email address")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "email address must be between 2 and 50 characters long")]
        public string? Email { get; set; }

        [Phone(ErrorMessage = "Invalid phone number")]
        public string? PhoneNumber { get; set; }

        public string? Description { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "category id must be greater than 1")]
        public int CategoryId { get; set; }

        public string? Image { get; set; }
    }
}
