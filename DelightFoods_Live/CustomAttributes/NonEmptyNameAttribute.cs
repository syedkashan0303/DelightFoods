using System.ComponentModel.DataAnnotations;

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
            }

            return ValidationResult.Success;
        }
    }
}
