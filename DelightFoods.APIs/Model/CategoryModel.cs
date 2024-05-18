using System.ComponentModel.DataAnnotations;

namespace DelightFoods.APIs.Model
{
    public class CategoryModel
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }

        [Required]
        public string Description { get; set; }
        public int ParentCategoryId { get; set; }
        public DateTime CreatedByUTC { get; set; } = DateTime.UtcNow;
    }
}
