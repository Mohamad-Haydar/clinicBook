using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace api.library.Attributes
{
    internal class StartLessThanEndHourAttribute : ValidationAttribute
    {
        private readonly string _comparisonProperty;

        public StartLessThanEndHourAttribute(string comparisonProperty)
        {
            _comparisonProperty = comparisonProperty;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var currentValue = (TimeSpan)value;

            var property = validationContext.ObjectType.GetProperty(_comparisonProperty);

            if (property == null)
                throw new ArgumentException("Property with this name not found");

            var comparisonValue = (TimeSpan)property.GetValue(validationContext.ObjectInstance);

            if (currentValue >= comparisonValue)
            {
                return new ValidationResult(ErrorMessage ?? "Start hour must be less than end hour");
            }

            return ValidationResult.Success;
        }
    }
}
