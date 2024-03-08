using System.ComponentModel.DataAnnotations;

namespace DelightFoods_Live.CustomAttributes
{
    public class PasswordStrengthAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            if (value == null || !(value is string))
            {
                return false;
            }

            string password = (string)value;

            if (password.Length < 8)
            {
                ErrorMessage = "Password must be at least 8 characters long.";
                return false;
            }

            if (!password.Any(char.IsUpper))
            {
                ErrorMessage = "Password must contain at least one uppercase letter.";
                return false;
            }

            if (!password.Any(char.IsLower))
            {
                ErrorMessage = "Password must contain at least one lowercase letter.";
                return false;
            }

            if (!password.Any(char.IsDigit))
            {
                ErrorMessage = "Password must contain at least one digit.";
                return false;
            }

            if (!password.Any(c => !char.IsLetterOrDigit(c)))
            {
                ErrorMessage = "Password must contain at least one special character.";
                return false;
            }

            return true;
        }
    }
}
