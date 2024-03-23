namespace DelightFoods_Live.Models
{
    public class MediaGalleryModel
    {

        public int Id { get; set; }
        public int ProductId { get; set; }
        public int CategoryId { get; set; }
        public string? Name { get; set; }
        public string? FileType { get; set; }
        public string? FilePath { get; set; }
        public DateTime CreatedOnUTC { get; set; }

    }
}
