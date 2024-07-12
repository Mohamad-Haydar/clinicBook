using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Configuration;
using NpgsqlTypes;

namespace api.library.Models.Request;

public class DoctorServiceRequest 
{
    public int duration { get; set; }
    public string doctorid { get; set; }
    public int serviceid { get; set; }
}