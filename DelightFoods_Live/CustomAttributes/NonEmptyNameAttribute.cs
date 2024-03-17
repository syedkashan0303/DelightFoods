using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace DelightFoods_Live.CustomAttributes
{
    public class NonEmptyNameAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value != null)
            {
                string name = value.ToString().Trim(); // Trim to handle cases where there might be only spaces

                if (string.IsNullOrWhiteSpace(name))
                {
                    return new ValidationResult("Name cannot be empty or whitespace.");
                }

                // Check if name contains only alphabetic characters using regular expression
                //if (!Regex.IsMatch(name, @"^[a-zA-Z]+$"))
                //{
                //    return new ValidationResult("Name can only contain alphabetic characters.");
                //}
            }
            return ValidationResult.Success;
        }
    }
}
