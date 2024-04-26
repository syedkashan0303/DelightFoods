namespace DelightFoods_Live.Models
{
	public class ReturnOrderModel
	{
		public int Id { get; set; }
		public int OrderId { get; set; }
		public int CustomerID { get; set; }
		public string Reason { get; set; }
		public bool IsApproved { get; set; }
		public DateTime CreatedOnUTC { get; set; }

	}
}
