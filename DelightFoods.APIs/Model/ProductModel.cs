using System.ComponentModel.DataAnnotations;

namespace DelightFoods.APIs.Model
{
    public class ProductModel
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Name")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "DescriptionHEre")]
        public string Description { get; set; }

        [Required]
        public int CategoryId { get; set; }

        [Range(1, 500, ErrorMessage = "The Stock must be a positive number or greator than 0.")]
        public int Stock { get; set; }
        public bool IsActive { get; set; } = true;

        [Range(1, 99999, ErrorMessage = "The Price must be a positive number or greator than 0.")]
        public decimal Price { get; set; }
        public DateTime CreatedOnUTC { get; set; }

        //Relationship 


        public CategoryModel? Category { get; set; } = default;
    }
}
