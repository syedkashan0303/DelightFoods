namespace DelightFoods_Live.Models
{
    public class PaymentModel
    {
        public int Id { get; set; }
        public string CardNumber { get; set; }
        public string Expiry { get; set; }
        public int CVC { get; set; }
    }
}
