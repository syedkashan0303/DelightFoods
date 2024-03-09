using System;
using System.ComponentModel.DataAnnotations;

namespace DelightFoods_Live.CustomAttributes
{
    public class NumericPhoneNumberAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value != null)
            {
                string phoneNumber = value.ToString();

                foreach (char c in phoneNumber)
                {
                    if (!char.IsDigit(c))
                    {
                        return new ValidationResult("Phone number must contain only numeric digits.");
                    }
                }

                // Additional validation if needed
                // Example: Check for a specific length of the phone number
                // if (phoneNumber.Length != 10)
                // {
                //     return new ValidationResult("Phone number must be 10 digits long.");
                // }
            }

            return ValidationResult.Success;
        }
    }
}
