namespace DelightFoods.APIs.Model
{
    public class PaymentTransaction
    {
        public int Id { get; set; }

        public bool IsCOD { get; set; }

        public int OrderId { get; set; }

        public decimal Amount { get; set; }

        public DateTime CreatedOnUTC { get; set; }
    }
}
