namespace DelightFoods_Live.Models.DTO
{
    public class SaleOrderDTO
    {
        public int Id { get; set; }
        public string ProductIds { get; set; }
        public int CustomerId { get; set; }
        public int Quantity { get; set; }
        public decimal TotalPrice { get; set; }
        public int Status { get; set; }
        public int ShippingId { get; set; }
        public DateTime CreatedOnUTC { get; set; }

        public int ProductId { get; set; }
        public string ProductName { get; set; }

    }
}
