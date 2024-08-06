using static System.Runtime.InteropServices.JavaScript.JSType;

namespace api.Models.Responce
{
    public class MyReservationResponse
    {
        public string availabledate { get; set; }
        public int doctor_availability_id {get;set;}
        public string doctor_id {get;set;}
        public string start_time {get;set;}
        public string end_time {get;set;}
        public int id {get;set;}
        public bool is_done {get;set;}
        public string[] service_names {get;set;}
    }
}
