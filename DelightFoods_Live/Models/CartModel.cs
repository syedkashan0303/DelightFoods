using System.ComponentModel.DataAnnotations.Schema;

namespace DelightFoods_Live.Models
{
    [Table("Cart")]
    public class CartModel
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int CustomerId { get; set; }
        public int Quantity { get; set; }
        public DateTime CreatedOnUTC { get; set; }
    }
}
