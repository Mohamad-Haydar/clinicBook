using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace api.Attributes
{
    internal class ValidStartHourAttribute : ValidationAttribute
    {
        private readonly TimeSpan _start = new(7, 0, 0); // 7 AM
        private readonly TimeSpan _end = new(20, 0, 0); // 8 PM

        public override bool IsValid(object value)
        {
            if (value is TimeSpan time)
            {
                return time >= _start && time <= _end;
            }
            return false;
        }
    }
}
