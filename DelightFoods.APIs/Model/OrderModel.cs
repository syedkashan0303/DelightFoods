using System.ComponentModel.DataAnnotations;

namespace DelightFoods.APIs.Model
{
    public class OrderModel
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public int ProductId { get; set; }
        public int CustomerId { get; set; }
        public int Quantity { get; set; }
        public string Status { get; set; }
        public decimal Total { get; set; }
        public int PaymentMethodId { get; set; }
        public DateTime CreatedOnUTC { get; set; }
    }
}
