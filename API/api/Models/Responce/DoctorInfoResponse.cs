namespace api.Models.Responce;

public class DoctorInfoResponse
{
    public string Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public string Description { get; set; }
    public string CategoryName { get; set; }
    public string? Image { get; set; }  
    public IEnumerable<DoctorServiceResponse>? Services { get; set; } = null;  
}