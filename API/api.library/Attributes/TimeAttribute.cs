using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace api.library.Attributes
{
    [AttributeUsage(AttributeTargets.Property, Inherited = true)]
    public class TimeAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            var dateString = value as string;
            if (string.IsNullOrWhiteSpace(dateString))
            {
                return true;
            }
            TimeOnly result;
            var success = TimeOnly.TryParse(dateString, out result);
            return success;
        }
    }
}
