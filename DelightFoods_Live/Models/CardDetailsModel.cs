using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace DelightFoods_Live.Models
{
    public class CardDetailsModel
    {
        public int Id { get; set; }

        public string CardholderName { get; set; }

        public string CardNumber { get; set; }

        public string Expiry { get; set; }

        public int CVC { get; set; }

        public int PaymentId { get; set; }

        public bool IsSave { get; set; }

        public DateTime CreatedOnUTC { get; set; }
    }
}
