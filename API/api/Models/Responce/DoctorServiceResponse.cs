using System.ComponentModel.DataAnnotations.Schema;

namespace api.Models.Responce
{
    public class DoctorServiceResponse
    {
        public int Id { get; set; }
        public int Duration { get; set; }
        public string DoctorId { get; set; }
        public int ServiceId { get; set; }
        public string ServiceName { get; set; }
    }
}
