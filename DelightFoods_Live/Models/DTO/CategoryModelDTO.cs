using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DelightFoods_Live.Models.DTO
{
    public class CategoryModelDTO
    {
        public CategoryModelDTO() 
        {
            SubCategoryList = new List<int>();
            ParentCategoryList = new List<SelectListItem>();
            MediaFileList = new List<MediaFiles>();
            UploadedFiles = new List<IFormFile>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int ParentCategoryId { get; set; }
        public DateTime CreatedByUTC { get; set; } = DateTime.UtcNow;

        public List<SelectListItem> ParentCategoryList { get; set; }
        public List<int> SubCategoryList { get; set; }

        [BindProperty]
        public IList<IFormFile> UploadedFiles { get; set; }

        public List<MediaFiles> MediaFileList { get; set; }

        public class MediaFiles
        {
            public byte[] FileBytes { get; set; }
            public string FileName { get; set; }
            public int FileId { get; set; }
        }

    }
}
