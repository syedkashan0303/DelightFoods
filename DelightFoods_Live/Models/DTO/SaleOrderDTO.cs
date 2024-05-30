namespace DelightFoods_Live.Models.DTO
{
    public class SaleOrderDTO
    {
        public SaleOrderDTO() 
        {
            cartDTOs = new List<CartDTO>();
            saleOrderProductMappings = new List<SaleOrderProductMappingDTO>();
        }   
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int CustomerId { get; set; }
        public string CustomerName { get; set; }
        public int Quantity { get; set; }
        public decimal TotalPrice { get; set; }
        public decimal AdvancePayment { get; set; }
        public decimal RemainingPayment { get; set; }
        public string Status { get; set; }
        public string Reason { get; set; }
        public string ReturnDate { get; set; }
        public int ShippingId { get; set; }
        public DateTime CreatedOnUTC { get; set; }
        public string CreatedStringDate { get; set; }
        public string ProductName { get; set; }
        public string ShippingAddress { get; set; }
        public int PaymentId { get; set; }
        public bool Cashondelivery { get; set; }
        public List<CartDTO> cartDTOs { get; set; }
        public List<SaleOrderProductMappingDTO> saleOrderProductMappings { get; set; }

        public string TexRate { get; set; }
        public string WithHoldingTex { get; set; }

        public string CardholderName { get; set; }
		public string CardNumber { get; set; }
		public string Expiry { get; set; }
		public int CVC { get; set; }

        public bool IsReturnDateIsValde { get; set; }

	}
}
