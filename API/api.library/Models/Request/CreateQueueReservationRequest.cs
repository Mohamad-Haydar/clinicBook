namespace api.library.Models.Request;

public class CreateQueueReservationRequest
{
    #pragma warning disable IDE1006
    public required string client_id { get; set; }
    public required int doctor_availability_id { get; set; }
    public required IList<int> doctor_service_ids { get; set; }
    #pragma warning restore IDE1006
}