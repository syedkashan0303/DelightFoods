using DelightFoods_Live.CustomAttributes;
using System.ComponentModel.DataAnnotations;

namespace DelightFoods_Live.Models
{
    public class CategoryModel
    {
        public int Id { get; set; }
        [Required]
        [NonEmptyName(ErrorMessage = "Name cannot be empty or whitespace.")]
        public string Name { get; set; }

        [Required]
        [NonEmptyName(ErrorMessage = "Description cannot be empty or whitespace.")]
        public string Description { get; set; }
        public int ParentCategoryId { get; set; }
        public DateTime CreatedByUTC { get; set; } = DateTime.UtcNow;
    }
}
