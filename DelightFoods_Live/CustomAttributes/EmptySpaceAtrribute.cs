using System;
using System.ComponentModel.DataAnnotations;

namespace DelightFoods_Live.CustomAttributes
{
    public class EmptySpaceAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value != null)
            {
                string input = value.ToString();

                // Check if the input consists only of whitespace
                if (string.IsNullOrWhiteSpace(input))
                {
                    return new ValidationResult(ErrorMessage);
                }
            }
            return ValidationResult.Success;
        }
    }
}
