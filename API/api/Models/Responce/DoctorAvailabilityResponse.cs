namespace api.Models.Responce
{
    public class DoctorAvailabilityResponse
    {
        public int id { get; set; }
        public DateOnly day {get;set;}
        public string dayName {get;set;}
        public TimeSpan startHour {get;set;}
        public TimeSpan endHour {get;set;}
        public int maxClient {get;set;}
    }
}
