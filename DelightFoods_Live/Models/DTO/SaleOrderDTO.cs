namespace DelightFoods_Live.Models.DTO
{
    public class SaleOrderDTO
    {
        public SaleOrderDTO() 
        {
            cartDTOs = new List<CartDTO>();
        }   
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int CustomerId { get; set; }
        public int Quantity { get; set; }
        public decimal TotalPrice { get; set; }
        public string Status { get; set; }
        public int ShippingId { get; set; }
        public DateTime CreatedOnUTC { get; set; }
        public string ProductName { get; set; }

        public List<CartDTO> cartDTOs { get; set; }

    }
}
