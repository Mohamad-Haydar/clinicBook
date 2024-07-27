using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Configuration;
using NpgsqlTypes;

namespace api.Models.Request;

public class DoctorServiceRequest 
{
    [Required(ErrorMessage = "Duration is required"),
     Range(minimum: 1, maximum: double.MaxValue, ErrorMessage = "Duration is required and should be greater than 0")]
    public int duration { get; set; }

    [Required(ErrorMessage = "Doctor Id is required")]
    public string doctorid { get; set; }
    
    [Required(ErrorMessage = "Service Id is required"),
     Range(minimum: 1, maximum: double.MaxValue, ErrorMessage = "service id is required and should be greater than 0")]
    public int serviceid { get; set; }
}