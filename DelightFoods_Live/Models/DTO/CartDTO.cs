namespace DelightFoods_Live.Models.DTO
{
    public class CartDTO
    {

        public int Id { get; set; }
        public int ProductId { get; set; }
        public int CustomerId { get; set; }
        public int Quantity { get; set; }
        public string ProductName { get; set; }
        public decimal ProductPrice { get; set; }
        public DateTime CreatedOnUTC { get; set; }
    }
}
