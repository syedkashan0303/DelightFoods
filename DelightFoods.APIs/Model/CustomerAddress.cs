namespace DelightFoods.APIs.Model
{
    public class CustomerAddress
    {
        public int Id { get; set; }
        public string DeliveryAddress { get; set; }
        public string BillingAddress { get; set; }
        public int CityId { get; set; }
        public DateTime CreatedByUTC { get; set; }
    }
}
