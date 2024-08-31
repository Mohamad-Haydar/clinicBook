namespace api.Models
{
    public class CookieUserModel
    {
        public string id { get; set; }
        public string userName {get; set;}
        public string email {get; set;}
        public string phoneNumber {get; set;}
        public IEnumerable<string> roles {get; set;}
    }
}
