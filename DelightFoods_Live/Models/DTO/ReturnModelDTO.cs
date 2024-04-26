namespace DelightFoods_Live.Models.DTO
{
	public class ReturnModelDTO
	{
		public int Id { get; set; }
		public int OrderId { get; set; }
		public int CustomerID { get; set; }
		public string Reason { get; set; }
		public bool IsApproved { get; set; }
		public DateTime CreatedOnUTC { get; set; }
		public decimal TotalPrice { get; set; }
		public string Status { get; set; }

	}
}
