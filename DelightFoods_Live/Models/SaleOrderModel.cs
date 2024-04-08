using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DelightFoods_Live.Models
{

    [Table("SaleOrder")]
    public class SaleOrderModel
    {

        [Key]
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public decimal TotalPrice { get; set; }
        public string Status { get; set; }
        public int ShippingId { get; set; }
        public DateTime CreatedOnUTC { get; set; }
    }
}
