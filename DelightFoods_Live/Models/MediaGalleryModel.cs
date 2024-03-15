namespace DelightFoods_Live.Models
{
    public class MediaGalleryModel
    {
        public int Id { get; set; }
        public int CaseId { get; set; }
        public int UserId { get; set; }
        public string Name { get; set; }
        public string FileType { get; set; }
        public string FilePath { get; set; }
        public DateTime CreatedOnUTC { get; set; }
    }
}
