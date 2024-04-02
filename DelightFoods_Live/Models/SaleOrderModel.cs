namespace DelightFoods_Live.Models
{
    public class SaleOrderModel
    {
        public int Id { get; set; }
        public int PaymentId { get; set; }
        public int CustomerId { get; set; }
        public decimal TotalPrice { get; set; }
        public string Status { get; set; }
        public int ShippingId { get; set; }
        public DateTime CreatedOnUTC { get; set; }
    }
}
