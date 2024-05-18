using System.ComponentModel.DataAnnotations;

namespace DelightFoods.APIs.Model
{
    public class CustomerModel
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        [Phone]
        [StringLength(11, ErrorMessage = "Your mobile number is invalid", MinimumLength = 11)]
        public string Mobile { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        public int AddressId { get; set; }
        public int PaymentDetailId { get; set; }
        public string UserId { get; set; }
        public DateTime CreatedByUTC { get; set; }
    }
}
