using System.ComponentModel.DataAnnotations;

namespace api.library.Attributes
{
    public class DateInTheFutureAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            if (value is DateOnly date)
            {
                return date >= DateOnly.FromDateTime(DateTime.Today);
            }
            return false;
        }
    }
}
