using api.Attributes;
using System.ComponentModel.DataAnnotations;

namespace api.Models.Request
{
    public class OpenAvailableDateRequest
    {
        [Required(ErrorMessage = "AvailableDate is required")]
        [Date(ErrorMessage = "Invalid Time format, the format should be yyyy-mm-dd")]
        [DateInTheFuture(ErrorMessage = "Date must be in the future")]
        public DateOnly AvailableDate { get; set; }

        [Required(ErrorMessage = "StartHour is required")]
        [ValidStartHour(ErrorMessage = "Start hour must be between 7 AM and 8 PM")]
        [StartLessThanEndHour("EndHour", ErrorMessage = "Start hour must be less than end hour")]
        [Time( ErrorMessage = "Invalid Time format, the format should be hh-mm-ss")]
        public TimeSpan StartHour { get; set; }

        [Required(ErrorMessage = "EndHour is required")]
        [ValidStartHour(ErrorMessage = "End hour must be between 7 AM and 8 PM")]
        [Time( ErrorMessage = "Invalid Time format, the format should be hh-mm-ss")]
        public TimeSpan EndHour { get; set; }

        [Required(ErrorMessage = "MaxClient is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Number of people must be greater than 0")]
        public int MaxClient { get; set; }

        [Required(ErrorMessage = "DoctorId is required")]
        public string DoctorId { get; set; }
        public int? RepetitionDelay { get; set; }
        [Required(ErrorMessage = "Nb Of Open Availability is required")]
        public int NbOfOpenAvailability { get; set; }
    }
}