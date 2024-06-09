using System.ComponentModel.DataAnnotations;

namespace DelightFoods.APIs.Model
{
    public class SaleOrderModel
    {
        [Key]
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public string Status { get; set; }
        public string? address { get; set; }
        public int ShippingId { get; set; }
        public DateTime CreatedOnUTC { get; set; }
        public decimal TotalPrice { get; set; }
    }
}
