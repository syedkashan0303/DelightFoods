namespace DelightFoods.APIs.Model
{
    public class SaleOrderProductMappingModel
    {
        public int Id { get; set; }
        public int ProductID { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public int SaleOrderId { get; set; }
    }
}
