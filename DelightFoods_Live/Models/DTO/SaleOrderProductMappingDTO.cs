namespace DelightFoods_Live.Models.DTO
{
    public class SaleOrderProductMappingDTO
    {
        public int Id { get; set; }
        public int ProductID { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public int SaleOrderId { get; set; }
        public string ProductName { get; set; }
        
    }
}
