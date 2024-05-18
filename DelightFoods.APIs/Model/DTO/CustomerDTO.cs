namespace DelightFoods.APIs.Model.DTO
{
    public class CustomerDTO
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Mobile { get; set; }
        public string Email { get; set; }
        public int CityId { get; set; }
        public string CityName { get; set; }
        public int AddressId { get; set; }
        public int PaymentDetailId { get; set; }
        public string UserId { get; set; }
        public DateTime CreatedByUTC { get; set; }
    }
}
