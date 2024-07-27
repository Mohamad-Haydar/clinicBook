using api.Attributes;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace api.Models.Request;

public class UpdateAvailableDateRequest
{
    [Required(ErrorMessage = "Id is required"),
     Range(minimum: 1, maximum: double.MaxValue, ErrorMessage = "Id is required and it should be greater than 0")]
    public int Id { get; set; }

    [Required(ErrorMessage = "AvailableDate is required")]
    [DataType(DataType.Date, ErrorMessage = "Invalid date format, the format should be yyyy-mm-dd")]
    [DateInTheFuture(ErrorMessage = "Date must be in the future")]
    public DateOnly AvailableDate { get; set; }

    [Required(ErrorMessage = "StartHour is required")]
    [ValidStartHour(ErrorMessage = "Start hour must be between 7 AM and 8 PM")]
    [StartLessThanEndHour("EndHour", ErrorMessage = "Start hour must be less than end hour")]
    public TimeSpan StartHour { get; set; }

    [Required(ErrorMessage = "EndHour is required")]
    [ValidStartHour(ErrorMessage = "End hour must be between 7 AM and 8 PM")]
    public TimeSpan EndHour { get; set; }

    [Required(ErrorMessage = "MaxClient is required")]
    [Range(1, int.MaxValue, ErrorMessage = "Number of people must be greater than 0")]
    public int MaxClient { get; set; }
}