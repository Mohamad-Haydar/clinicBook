using System.ComponentModel.DataAnnotations;

namespace api.Models.Request;

public class CreateQueueReservationRequest
{
    #pragma warning disable IDE1006
    [Required(ErrorMessage = "client_id is required")]
    public string client_id { get; set; }

    [Required(ErrorMessage = "doctor_availability_id is required"), 
    Range(1, maximum: double.MaxValue, ErrorMessage = "Enter a valid availability")]
    public int? doctor_availability_id { get; set; }
    
    [Required(ErrorMessage = "doctor_service_ids is required")]
    public IList<int> doctor_service_ids { get; set; }
    #pragma warning restore IDE1006
}