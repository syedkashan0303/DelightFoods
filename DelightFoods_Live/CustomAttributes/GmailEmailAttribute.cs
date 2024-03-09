using System.ComponentModel.DataAnnotations;

namespace DelightFoods_Live.CustomAttributes
{
    public class GmailEmailAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value != null)
            {
                string email = value.ToString().ToLower(); // Convert to lower case for case-insensitive comparison

                if (!email.EndsWith("@gmail.com"))
                {
                    return new ValidationResult("Email must be a Gmail address.");
                }
            }

            return ValidationResult.Success;
        }
    }
}
