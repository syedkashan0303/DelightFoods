namespace DelightFoods_Live.Models.DTO
{
    public class CartDTO 
    {

        public CartDTO()
        {
            //paymentModel = new PaymentModel();
            CartDTOlist = new List<CartDTO>();
        }
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int CustomerId { get; set; }
        public int Quantity { get; set; }
        public string ProductName { get; set; }
        public string MediaFilePath { get; set; }
        public decimal ProductPrice { get; set; }
        public decimal TotalPrice { get; set; }
        public DateTime CreatedOnUTC { get; set; }
        public bool IsOrderCreated { get; set; }
        //public PaymentModel paymentModel { get; set; }

        public List<CartDTO> CartDTOlist {  get; set; }

        public string CardNumber { get; set; }
        public string Expiry { get; set; }
        public int CVC { get; set; }
    }
}
