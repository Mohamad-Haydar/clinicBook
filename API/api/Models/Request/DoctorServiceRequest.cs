using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Configuration;
using NpgsqlTypes;

namespace api.Models.Request;

public class DoctorServiceRequest 
{
    [Required(ErrorMessage = "Duration is required")]
    public int duration { get; set; }

    [Required(ErrorMessage = "Doctor Id is required")]
    public string doctorid { get; set; }
    
    [Required(ErrorMessage = "Service Id is required")]
    public int serviceid { get; set; }
}