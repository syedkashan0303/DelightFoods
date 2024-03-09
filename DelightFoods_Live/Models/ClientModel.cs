using DelightFoods_Live.CustomAttributes;
using System.ComponentModel.DataAnnotations;

namespace DelightFoods_Live.Models
{
    public class CustomerModel
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [NonEmptyName(ErrorMessage = "Name cannot be empty or whitespace.")]
        public string FirstName { get; set; }

        [Required]
        [NonEmptyName(ErrorMessage = "Name cannot be empty or whitespace.")]
        public string LastName { get; set; }

        [Required]
        [Phone]
        [StringLength(11, ErrorMessage ="Your mobile number is invalid", MinimumLength = 11)]
        [NumericPhoneNumber(ErrorMessage = "Invalid phone number.")]
        public string Mobile { get; set; }

        [Required]
        [EmailAddress]
        [GmailEmail(ErrorMessage = "Email must be a Gmail address.")]
        public string Email { get; set; }
    }
}
