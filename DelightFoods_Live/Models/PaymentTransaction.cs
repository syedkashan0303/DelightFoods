using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DelightFoods_Live.Models
{
    public class PaymentTransaction
    {
        public int Id { get; set; }
    
        public bool IsCOD { get; set; }

        public int OrderId { get; set; }

        public DateTime CreatedOnUTC { get; set; }

    }
}
