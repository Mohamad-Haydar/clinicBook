using System.ComponentModel.DataAnnotations;

namespace api.Models.Request;

public class CreateQueueReservationRequest
{
    #pragma warning disable IDE1006
    [Required(ErrorMessage = "Name is required")]
    public string client_id { get; set; }

    [Required(ErrorMessage = "Name is required")]
    public int doctor_availability_id { get; set; }
    
    [Required(ErrorMessage = "Name is required")]
    public IList<int> doctor_service_ids { get; set; }
    #pragma warning restore IDE1006
}