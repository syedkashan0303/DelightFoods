namespace DelightFoods_Live.Models
{
    public class CategoryModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime CreatedByUTC { get; set; } = DateTime.UtcNow;
    }
}
