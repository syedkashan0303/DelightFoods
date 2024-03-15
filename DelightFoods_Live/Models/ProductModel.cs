namespace DelightFoods_Live.Models
{
    public class ProductModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Deception { get; set; }
        public int CategoryId { get; set; }
        public int Stock { get; set; }
        public bool IsActive { get; set; }
        public decimal Price { get; set; }
        public DateTime CreatedOnUTC { get; set; }
    }
}
