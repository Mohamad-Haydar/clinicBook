using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using NpgsqlTypes;

namespace api.library.Models;

public class DoctorServiceRequest 
{
    public int duration { get; set; }
    public string doctorid { get; set; }
    public int serviceid { get; set; }
}